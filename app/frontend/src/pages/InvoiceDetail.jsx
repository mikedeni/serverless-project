import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../utils/api';

const statusColors = {
    draft: { bg: 'rgba(148,163,184,0.2)', color: '#94A3B8' },
    sent: { bg: 'rgba(59,130,246,0.2)', color: '#60A5FA' },
    paid: { bg: 'rgba(16,185,129,0.2)', color: '#34D399' },
    overdue: { bg: 'rgba(239,68,68,0.2)', color: '#FCA5A5' },
    cancelled: { bg: 'rgba(107,114,128,0.15)', color: '#6B7280' }
};

const methodLabels = {
    cash: '💵 Cash',
    transfer: '🏦 Transfer',
    cheque: '📝 Cheque',
    other: '📋 Other'
};

const InvoiceDetail = () => {
    const { id } = useParams();
    const [invoice, setInvoice] = useState(null);
    const [loading, setLoading] = useState(true);
    const [showPayment, setShowPayment] = useState(false);
    const [paying, setPaying] = useState(false);
    const [downloading, setDownloading] = useState(false);
    const [error, setError] = useState('');
    const [statusMsg, setStatusMsg] = useState('');

    const [paymentForm, setPaymentForm] = useState({
        amount: '',
        paymentDate: new Date().toISOString().split('T')[0],
        method: 'transfer',
        note: ''
    });

    const fetchInvoice = async () => {
        try {
            setLoading(true);
            const data = await api.get(`/invoices/${id}`);
            setInvoice(data);
        } catch (err) {
            console.error('Failed to load invoice', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { fetchInvoice(); }, [id]);

    const handleRecordPayment = async (e) => {
        e.preventDefault();
        setPaying(true);
        setError('');
        try {
            await api.post(`/invoices/${id}/payments`, {
                ...paymentForm,
                amount: parseFloat(paymentForm.amount)
            });
            setShowPayment(false);
            setPaymentForm({ amount: '', paymentDate: new Date().toISOString().split('T')[0], method: 'transfer', note: '' });
            fetchInvoice();
        } catch (err) {
            setError(err.message);
        } finally {
            setPaying(false);
        }
    };

    const handleStatusChange = async (newStatus) => {
        try {
            setStatusMsg('');
            await api.put(`/invoices/${id}/status`, { status: newStatus });
            setStatusMsg(`Status updated to "${newStatus}"`);
            fetchInvoice();
            setTimeout(() => setStatusMsg(''), 3000);
        } catch (err) {
            setError(err.message);
        }
    };

    const handleDownloadPdf = async () => {
        setDownloading(true);
        try {
            const token = localStorage.getItem('token');
            const res = await fetch(`/api/reports/invoice/${id}/pdf`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            const blob = await res.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `Invoice_${invoice.invoiceNumber}.pdf`;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        } catch (err) {
            alert('Failed to download PDF');
        } finally {
            setDownloading(false);
        }
    };

    if (loading) return <div className="loader-container"><div className="spinner"></div></div>;
    if (!invoice) return <div className="page-container"><p>Invoice not found.</p></div>;

    const sc = statusColors[invoice.status] || statusColors.draft;
    const paidPercent = invoice.totalAmount > 0 ? Math.min((invoice.paidAmount / invoice.totalAmount) * 100, 100) : 0;

    return (
        <div className="page-container fadeIn">
            <header className="top-header detail-header">
                <div>
                    <Link to={`/projects/${invoice.projectId}/invoices`} className="breadcrumb">← Back to Invoices</Link>
                    <h1 style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                        {invoice.invoiceNumber}
                        <span style={{
                            padding: '6px 16px',
                            borderRadius: '20px',
                            background: sc.bg,
                            color: sc.color,
                            fontSize: '14px',
                            fontWeight: 600,
                            textTransform: 'uppercase'
                        }}>{invoice.status}</span>
                    </h1>
                </div>
                <div style={{ display: 'flex', gap: '8px' }}>
                    <button onClick={handleDownloadPdf} disabled={downloading} className="btn-secondary" style={{ width: 'auto', border: '1px solid rgba(165,180,252,0.4)', color: '#A5B4FC' }}>
                        {downloading ? '⏳ Generating...' : '📄 Download PDF'}
                    </button>
                    {invoice.status !== 'paid' && invoice.status !== 'cancelled' && (
                        <button className="btn-primary" style={{ width: 'auto' }} onClick={() => setShowPayment(!showPayment)}>
                            {showPayment ? '✕ Cancel' : '💳 Record Payment'}
                        </button>
                    )}
                </div>
            </header>

            {statusMsg && (
                <div style={{ 
                    padding: '12px 16px', marginBottom: '16px', borderRadius: '8px',
                    background: 'rgba(16,185,129,0.15)', border: '1px solid rgba(16,185,129,0.3)', color: '#34D399'
                }}>{statusMsg}</div>
            )}

            <div className="detail-grid">
                {/* Main Pane */}
                <div className="main-pane">
                    {/* Invoice Info */}
                    <div className="glass-panel" style={{ padding: '24px', marginBottom: '24px' }}>
                        <h2 className="section-title">Invoice Information</h2>
                        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                            <div className="detail-row" style={{ flexDirection: 'column' }}>
                                <span style={{ color: '#94A3B8', fontSize: '12px', textTransform: 'uppercase' }}>Client</span>
                                <span style={{ fontWeight: 600, fontSize: '16px' }}>{invoice.clientName}</span>
                            </div>
                            <div className="detail-row" style={{ flexDirection: 'column' }}>
                                <span style={{ color: '#94A3B8', fontSize: '12px', textTransform: 'uppercase' }}>Milestone</span>
                                <span style={{ fontWeight: 600, fontSize: '16px' }}>{invoice.milestoneLabel || '-'}</span>
                            </div>
                            <div className="detail-row" style={{ flexDirection: 'column' }}>
                                <span style={{ color: '#94A3B8', fontSize: '12px', textTransform: 'uppercase' }}>Due Date</span>
                                <span style={{ fontWeight: 600 }}>{invoice.dueDate ? new Date(invoice.dueDate).toLocaleDateString() : 'Not set'}</span>
                            </div>
                            <div className="detail-row" style={{ flexDirection: 'column' }}>
                                <span style={{ color: '#94A3B8', fontSize: '12px', textTransform: 'uppercase' }}>Created</span>
                                <span style={{ fontWeight: 600 }}>{new Date(invoice.createdAt).toLocaleDateString()}</span>
                            </div>
                        </div>
                        {invoice.description && (
                            <div style={{ marginTop: '16px', padding: '12px', background: 'rgba(0,0,0,0.2)', borderRadius: '8px', fontSize: '14px', color: '#94A3B8' }}>
                                {invoice.description}
                            </div>
                        )}
                    </div>

                    {/* Status Actions */}
                    {invoice.status !== 'paid' && invoice.status !== 'cancelled' && (
                        <div className="glass-panel" style={{ padding: '16px', marginBottom: '24px', display: 'flex', gap: '8px', flexWrap: 'wrap', alignItems: 'center' }}>
                            <span style={{ color: '#94A3B8', fontSize: '13px', marginRight: '8px' }}>Change Status:</span>
                            {invoice.status === 'draft' && (
                                <button className="btn-secondary" style={{ fontSize: '13px' }} onClick={() => handleStatusChange('sent')}>📤 Mark as Sent</button>
                            )}
                            {(invoice.status === 'sent' || invoice.status === 'draft') && (
                                <button className="btn-secondary" style={{ fontSize: '13px', borderColor: 'rgba(239,68,68,0.3)' }} onClick={() => handleStatusChange('overdue')}>⚠️ Mark Overdue</button>
                            )}
                            {invoice.status !== 'draft' && (
                                <button className="btn-secondary" style={{ fontSize: '13px' }} onClick={() => handleStatusChange('draft')}>↩ Back to Draft</button>
                            )}
                            <button className="btn-secondary" style={{ fontSize: '13px', borderColor: 'rgba(107,114,128,0.3)' }} onClick={() => handleStatusChange('cancelled')}>✕ Cancel Invoice</button>
                        </div>
                    )}

                    {/* Payment Form */}
                    {showPayment && (
                        <div className="glass-panel" style={{ padding: '24px', marginBottom: '24px', animation: 'fadeIn 0.3s ease' }}>
                            <h3 style={{ marginBottom: '16px' }}>💳 Record Payment</h3>
                            {error && <div className="auth-error">{error}</div>}
                            <form onSubmit={handleRecordPayment}>
                                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                                    <div className="input-group">
                                        <input type="number" step="0.01" placeholder=" " value={paymentForm.amount}
                                            onChange={e => setPaymentForm({...paymentForm, amount: e.target.value})} required />
                                        <label>Amount (฿) — Remaining: ฿{invoice.remainingBalance?.toLocaleString()}</label>
                                    </div>
                                    <div className="input-group">
                                        <input type="date" placeholder=" " value={paymentForm.paymentDate}
                                            onChange={e => setPaymentForm({...paymentForm, paymentDate: e.target.value})} required />
                                        <label className="active-label">Payment Date</label>
                                    </div>
                                    <div>
                                        <label style={{ color: '#94A3B8', fontSize: '12px', marginBottom: '6px', display: 'block' }}>Method</label>
                                        <div style={{ display: 'flex', gap: '8px', flexWrap: 'wrap' }}>
                                            {['transfer', 'cash', 'cheque', 'other'].map(m => (
                                                <button key={m} type="button"
                                                    onClick={() => setPaymentForm({...paymentForm, method: m})}
                                                    style={{
                                                        padding: '8px 14px', borderRadius: '8px', fontSize: '13px', cursor: 'pointer',
                                                        border: paymentForm.method === m ? '2px solid #4F46E5' : '1px solid rgba(255,255,255,0.1)',
                                                        background: paymentForm.method === m ? 'rgba(79,70,229,0.2)' : 'rgba(255,255,255,0.05)',
                                                        color: paymentForm.method === m ? '#A5B4FC' : '#94A3B8'
                                                    }}
                                                >{methodLabels[m]}</button>
                                            ))}
                                        </div>
                                    </div>
                                    <div className="input-group">
                                        <input type="text" placeholder=" " value={paymentForm.note}
                                            onChange={e => setPaymentForm({...paymentForm, note: e.target.value})} />
                                        <label>Note</label>
                                    </div>
                                </div>
                                <button type="submit" className="btn-primary" style={{ width: 'auto', marginTop: '12px' }} disabled={paying}>
                                    {paying ? 'Recording...' : 'Confirm Payment'}
                                </button>
                            </form>
                        </div>
                    )}

                    {/* Payment History */}
                    <h2 className="section-title">Payment History</h2>
                    <div className="glass-panel table-wrapper">
                        {(!invoice.payments || invoice.payments.length === 0) ? (
                            <p style={{ padding: '24px', color: '#94A3B8', textAlign: 'center' }}>No payments recorded yet.</p>
                        ) : (
                            <table className="modern-table">
                                <thead>
                                    <tr>
                                        <th>Date</th>
                                        <th>Method</th>
                                        <th>Amount</th>
                                        <th>Note</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {invoice.payments.map(p => (
                                        <tr key={p.id}>
                                            <td>{new Date(p.paymentDate).toLocaleDateString()}</td>
                                            <td>{methodLabels[p.method] || p.method}</td>
                                            <td style={{ color: '#34D399', fontWeight: 600 }}>+฿{p.amount?.toLocaleString()}</td>
                                            <td style={{ color: '#94A3B8' }}>{p.note || '-'}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        )}
                    </div>
                </div>

                {/* Side Pane */}
                <div className="side-pane">
                    <h2 className="section-title">Payment Summary</h2>
                    <div className="glass-panel summary-card">
                        <div className="summary-row">
                            <span>Subtotal</span>
                            <span style={{ fontWeight: 600 }}>฿{invoice.amount?.toLocaleString()}</span>
                        </div>
                        <div className="summary-row">
                            <span>Tax ({invoice.taxPercent}%)</span>
                            <span>฿{invoice.taxAmount?.toLocaleString()}</span>
                        </div>
                        <hr className="divider" />
                        <div className="summary-row large">
                            <span>Total</span>
                            <span className="highlight">฿{invoice.totalAmount?.toLocaleString()}</span>
                        </div>

                        <div style={{ marginTop: '24px' }}>
                            <div className="summary-row">
                                <span>Paid</span>
                                <span className="text-success" style={{ fontWeight: 600 }}>฿{invoice.paidAmount?.toLocaleString()}</span>
                            </div>
                            <div className="summary-row">
                                <span>Remaining</span>
                                <span className={invoice.remainingBalance > 0 ? 'text-danger' : 'text-success'} style={{ fontWeight: 600 }}>
                                    ฿{invoice.remainingBalance?.toLocaleString()}
                                </span>
                            </div>
                        </div>

                        {/* Progress Bar */}
                        <div style={{ marginTop: '16px' }}>
                            <div className="pc-bar-bg">
                                <div
                                    className={`pc-bar-fill ${paidPercent >= 100 ? 'bg-green' : 'bg-blue'}`}
                                    style={{ width: `${paidPercent}%` }}
                                ></div>
                            </div>
                            <p style={{ fontSize: '12px', color: '#94A3B8', textAlign: 'right', marginTop: '4px' }}>
                                {paidPercent.toFixed(0)}% Paid
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default InvoiceDetail;
