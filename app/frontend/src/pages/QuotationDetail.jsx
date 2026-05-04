import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import api from '../utils/api';

const statusColors = {
    draft: { bg: 'rgba(148, 163, 184, 0.2)', color: '#94A3B8' },
    sent: { bg: 'rgba(59, 130, 246, 0.2)', color: '#60A5FA' },
    approved: { bg: 'rgba(16, 185, 129, 0.2)', color: '#34D399' },
    rejected: { bg: 'rgba(239, 68, 68, 0.2)', color: '#FCA5A5' }
};

const statusFlow = {
    draft: ['sent'],
    sent: ['approved', 'rejected'],
    approved: [],
    rejected: ['draft']
};

const QuotationDetail = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const [detail, setDetail] = useState(null);
    const [loading, setLoading] = useState(true);
    const [downloading, setDownloading] = useState(false);
    const [addingItem, setAddingItem] = useState(false);
    const [newItem, setNewItem] = useState({ description: '', qty: 1, unit: 'ชิ้น', unitPrice: 0 });

    useEffect(() => {
        fetchDetail();
    }, [id]);

    const fetchDetail = async () => {
        try {
            const data = await api.get(`/quotations/${id}`);
            setDetail(data);
        } catch (err) {
            console.error('Failed to load quotation', err);
        } finally {
            setLoading(false);
        }
    };

    const handleStatusChange = async (newStatus) => {
        try {
            await api.put(`/quotations/${id}/status`, { status: newStatus });
            fetchDetail();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleAddItem = async () => {
        if (!newItem.description.trim()) return;
        try {
            await api.post(`/quotations/${id}/items`, newItem);
            setNewItem({ description: '', qty: 1, unit: 'ชิ้น', unitPrice: 0 });
            setAddingItem(false);
            fetchDetail();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleDeleteItem = async (itemId) => {
        if (!confirm('ลบรายการนี้?')) return;
        try {
            await api.delete(`/quotations/${id}/items/${itemId}`);
            fetchDetail();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleDelete = async () => {
        if (!confirm('ลบใบเสนอราคานี้ทั้งหมด?')) return;
        try {
            await api.delete(`/quotations/${id}`);
            navigate(-1);
        } catch (err) {
            alert(err.message);
        }
    };

    const handleDownloadPdf = async () => {
        setDownloading(true);
        try {
            const token = localStorage.getItem('token');
            const res = await fetch(`/api/reports/quotation/${id}/pdf`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            const blob = await res.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `Quotation_${detail.quotationNumber}.pdf`;
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
    if (!detail) return <div className="page-container">Quotation not found.</div>;

    const sc = statusColors[detail.status] || statusColors.draft;
    const nextStatuses = statusFlow[detail.status] || [];

    const inputStyle = {
        padding: '8px 12px', background: 'rgba(0,0,0,0.2)',
        border: '1px solid rgba(255,255,255,0.1)', borderRadius: '6px',
        color: '#fff', fontSize: '14px', outline: 'none'
    };

    return (
        <div className="page-container fadeIn">
            <header className="top-header">
                <div>
                    <Link to={`/projects/${detail.projectId}/quotations`} className="breadcrumb">← Back to Quotations</Link>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                        <h1>{detail.quotationNumber}</h1>
                        <span style={{ padding: '6px 14px', borderRadius: '8px', fontSize: '12px', fontWeight: 600, textTransform: 'uppercase', background: sc.bg, color: sc.color }}>
                            {detail.status}
                        </span>
                    </div>
                </div>
                <div style={{ display: 'flex', gap: '8px' }}>
                    <button onClick={handleDownloadPdf} disabled={downloading} className="btn-secondary" style={{ width: 'auto', border: '1px solid rgba(165,180,252,0.4)', color: '#A5B4FC' }}>
                        {downloading ? '⏳ Generating...' : '📄 Download PDF'}
                    </button>
                    {nextStatuses.map(s => (
                        <button key={s} onClick={() => handleStatusChange(s)} className="btn-primary" style={{ width: 'auto', fontSize: '13px', padding: '10px 20px', textTransform: 'capitalize' }}>
                            → {s}
                        </button>
                    ))}
                    <button onClick={handleDelete} className="btn-secondary" style={{ color: '#FCA5A5', borderColor: 'rgba(239,68,68,0.3)' }}>Delete</button>
                </div>
            </header>

            {/* Client Info */}
            <div className="glass-panel" style={{ padding: '24px', marginBottom: '24px' }}>
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '16px' }}>
                    <div>
                        <div style={{ fontSize: '12px', color: 'var(--text-muted)', textTransform: 'uppercase', marginBottom: '4px' }}>Client</div>
                        <div style={{ fontWeight: 600 }}>{detail.clientName}</div>
                    </div>
                    <div>
                        <div style={{ fontSize: '12px', color: 'var(--text-muted)', textTransform: 'uppercase', marginBottom: '4px' }}>Phone</div>
                        <div>{detail.clientPhone || '-'}</div>
                    </div>
                    <div>
                        <div style={{ fontSize: '12px', color: 'var(--text-muted)', textTransform: 'uppercase', marginBottom: '4px' }}>Valid Until</div>
                        <div>{detail.validUntil ? new Date(detail.validUntil).toLocaleDateString() : '-'}</div>
                    </div>
                    {detail.clientAddress && (
                        <div style={{ gridColumn: 'span 3' }}>
                            <div style={{ fontSize: '12px', color: 'var(--text-muted)', textTransform: 'uppercase', marginBottom: '4px' }}>Address</div>
                            <div>{detail.clientAddress}</div>
                        </div>
                    )}
                </div>
            </div>

            <div className="detail-grid">
                {/* Items Table */}
                <div>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
                        <h2 className="section-title" style={{ margin: 0 }}>BOQ Items</h2>
                        {detail.status === 'draft' && (
                            <button onClick={() => setAddingItem(!addingItem)} className="btn-secondary" style={{ fontSize: '13px' }}>
                                {addingItem ? 'Cancel' : '+ Add Item'}
                            </button>
                        )}
                    </div>
                    <div className="glass-panel table-wrapper">
                        <table className="modern-table">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Description</th>
                                    <th>Qty</th>
                                    <th>Unit</th>
                                    <th>Unit Price</th>
                                    <th>Amount</th>
                                    {detail.status === 'draft' && <th></th>}
                                </tr>
                            </thead>
                            <tbody>
                                {detail.items.map((item, idx) => (
                                    <tr key={item.id}>
                                        <td style={{ color: 'var(--text-muted)' }}>{idx + 1}</td>
                                        <td>{item.description}</td>
                                        <td>{item.qty}</td>
                                        <td>{item.unit}</td>
                                        <td>฿{item.unitPrice.toLocaleString()}</td>
                                        <td style={{ fontWeight: 600, color: '#34D399' }}>฿{item.amount.toLocaleString()}</td>
                                        {detail.status === 'draft' && (
                                            <td>
                                                <button onClick={() => handleDeleteItem(item.id)} style={{ background: 'none', border: 'none', color: '#FCA5A5', cursor: 'pointer' }}>✕</button>
                                            </td>
                                        )}
                                    </tr>
                                ))}
                                {addingItem && (
                                    <tr>
                                        <td style={{ color: 'var(--text-muted)' }}>+</td>
                                        <td><input style={{ ...inputStyle, width: '100%' }} value={newItem.description} onChange={(e) => setNewItem({ ...newItem, description: e.target.value })} placeholder="รายละเอียด" /></td>
                                        <td><input style={{ ...inputStyle, width: '60px' }} type="number" value={newItem.qty} onChange={(e) => setNewItem({ ...newItem, qty: parseFloat(e.target.value) || 0 })} /></td>
                                        <td><input style={{ ...inputStyle, width: '60px' }} value={newItem.unit} onChange={(e) => setNewItem({ ...newItem, unit: e.target.value })} /></td>
                                        <td><input style={{ ...inputStyle, width: '90px' }} type="number" value={newItem.unitPrice} onChange={(e) => setNewItem({ ...newItem, unitPrice: parseFloat(e.target.value) || 0 })} /></td>
                                        <td style={{ fontWeight: 600, color: '#34D399' }}>฿{(newItem.qty * newItem.unitPrice).toLocaleString()}</td>
                                        <td><button onClick={handleAddItem} className="btn-primary" style={{ padding: '6px 12px', fontSize: '12px', width: 'auto' }}>Add</button></td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>

                {/* Summary */}
                <div>
                    <h2 className="section-title">Price Summary</h2>
                    <div className="glass-panel summary-card">
                        <div className="summary-row"><span>SubTotal</span><span>฿{detail.summary.subTotal.toLocaleString()}</span></div>
                        {detail.summary.markupAmount > 0 && <div className="summary-row"><span>Markup ({detail.markupPercent}%)</span><span className="text-success">+฿{detail.summary.markupAmount.toLocaleString()}</span></div>}
                        {detail.summary.discountAmount > 0 && <div className="summary-row"><span>Discount</span><span className="text-danger">-฿{detail.summary.discountAmount.toLocaleString()}</span></div>}
                        <div className="summary-row"><span>Tax ({detail.taxPercent}%)</span><span>฿{detail.summary.taxAmount.toLocaleString()}</span></div>
                        <hr className="divider" />
                        <div className="summary-row large">
                            <span>Grand Total</span>
                            <span style={{ background: 'linear-gradient(to right, #34D399, #10B981)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent' }}>
                                ฿{detail.summary.grandTotal.toLocaleString()}
                            </span>
                        </div>

                        {detail.note && (
                            <>
                                <hr className="divider" />
                                <div style={{ fontSize: '13px', color: 'var(--text-muted)' }}>
                                    <strong>Note:</strong> {detail.note}
                                </div>
                            </>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default QuotationDetail;
