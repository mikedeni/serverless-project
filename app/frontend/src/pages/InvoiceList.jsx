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

const InvoiceList = () => {
    const { projectId } = useParams();
    const [invoices, setInvoices] = useState([]);
    const [loading, setLoading] = useState(true);
    const [filter, setFilter] = useState('');
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [showForm, setShowForm] = useState(false);
    const [creating, setCreating] = useState(false);
    const [error, setError] = useState('');

    const [form, setForm] = useState({
        projectId: parseInt(projectId),
        clientName: '',
        description: '',
        amount: '',
        taxPercent: 7,
        dueDate: '',
        milestoneLabel: ''
    });

    const fetchInvoices = async () => {
        try {
            setLoading(true);
            const params = new URLSearchParams({ page, pageSize: 10 });
            if (filter) params.append('status', filter);
            const data = await api.get(`/invoices/project/${projectId}?${params}`);
            setInvoices(data.items || []);
            setTotalPages(data.totalPages || 1);
        } catch (err) {
            console.error('Failed to load invoices', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { fetchInvoices(); }, [projectId, filter, page]);

    const handleCreate = async (e) => {
        e.preventDefault();
        setCreating(true);
        setError('');
        try {
            await api.post('/invoices', {
                ...form,
                amount: parseFloat(form.amount),
                taxPercent: parseFloat(form.taxPercent)
            });
            setShowForm(false);
            setForm({ projectId: parseInt(projectId), clientName: '', description: '', amount: '', taxPercent: 7, dueDate: '', milestoneLabel: '' });
            fetchInvoices();
        } catch (err) {
            setError(err.message);
        } finally {
            setCreating(false);
        }
    };

    return (
        <div className="page-container fadeIn">
            <header className="top-header detail-header">
                <div>
                    <Link to={`/projects/${projectId}`} className="breadcrumb">← Back to Project</Link>
                    <h1>💰 Invoices</h1>
                </div>
                <button className="btn-primary" style={{ width: 'auto' }} onClick={() => setShowForm(!showForm)}>
                    {showForm ? '✕ Cancel' : '+ New Invoice'}
                </button>
            </header>

            {/* Status Filter */}
            <div style={{ display: 'flex', gap: '8px', marginBottom: '24px', flexWrap: 'wrap' }}>
                {['', 'draft', 'sent', 'paid', 'overdue', 'cancelled'].map(s => (
                    <button
                        key={s}
                        onClick={() => { setFilter(s); setPage(1); }}
                        style={{
                            padding: '8px 16px',
                            borderRadius: '20px',
                            border: filter === s ? '2px solid #4F46E5' : '1px solid rgba(255,255,255,0.1)',
                            background: filter === s ? 'rgba(79,70,229,0.2)' : 'rgba(255,255,255,0.05)',
                            color: filter === s ? '#A5B4FC' : '#94A3B8',
                            cursor: 'pointer',
                            fontSize: '13px',
                            fontWeight: 600,
                            textTransform: 'uppercase',
                            transition: 'all 0.2s'
                        }}
                    >
                        {s || 'All'}
                    </button>
                ))}
            </div>

            {/* Create Form */}
            {showForm && (
                <div className="glass-panel" style={{ padding: '24px', marginBottom: '24px', animation: 'fadeIn 0.3s ease' }}>
                    <h3 style={{ marginBottom: '16px' }}>Create Invoice</h3>
                    {error && <div className="auth-error">{error}</div>}
                    <form onSubmit={handleCreate}>
                        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                            <div className="input-group">
                                <input type="text" placeholder=" " value={form.clientName}
                                    onChange={e => setForm({...form, clientName: e.target.value})} required />
                                <label>Client Name</label>
                            </div>
                            <div className="input-group">
                                <input type="text" placeholder=" " value={form.milestoneLabel}
                                    onChange={e => setForm({...form, milestoneLabel: e.target.value})} />
                                <label>Milestone (e.g. งวดที่ 1)</label>
                            </div>
                            <div className="input-group">
                                <input type="number" step="0.01" placeholder=" " value={form.amount}
                                    onChange={e => setForm({...form, amount: e.target.value})} required />
                                <label>Amount (฿)</label>
                            </div>
                            <div className="input-group">
                                <input type="number" step="0.01" placeholder=" " value={form.taxPercent}
                                    onChange={e => setForm({...form, taxPercent: e.target.value})} />
                                <label>Tax %</label>
                            </div>
                            <div className="input-group">
                                <input type="date" placeholder=" " value={form.dueDate}
                                    onChange={e => setForm({...form, dueDate: e.target.value})} />
                                <label className="active-label">Due Date</label>
                            </div>
                            <div className="input-group">
                                <input type="text" placeholder=" " value={form.description}
                                    onChange={e => setForm({...form, description: e.target.value})} />
                                <label>Description</label>
                            </div>
                        </div>
                        <button type="submit" className="btn-primary" style={{ width: 'auto', marginTop: '8px' }} disabled={creating}>
                            {creating ? 'Creating...' : 'Create Invoice'}
                        </button>
                    </form>
                </div>
            )}

            {/* Invoice Table */}
            {loading ? (
                <div className="loader-container" style={{ height: '200px' }}><div className="spinner"></div></div>
            ) : invoices.length === 0 ? (
                <div className="glass-panel" style={{ padding: '40px', textAlign: 'center', color: '#94A3B8' }}>
                    No invoices found. Create one to start billing.
                </div>
            ) : (
                <>
                    <div className="glass-panel table-wrapper">
                        <table className="modern-table">
                            <thead>
                                <tr>
                                    <th>Invoice #</th>
                                    <th>Client</th>
                                    <th>Milestone</th>
                                    <th>Amount</th>
                                    <th>Total (incl. Tax)</th>
                                    <th>Due Date</th>
                                    <th>Status</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {invoices.map(inv => {
                                    const sc = statusColors[inv.status] || statusColors.draft;
                                    return (
                                        <tr key={inv.id} style={{ cursor: 'pointer' }}>
                                            <td style={{ fontWeight: 600, fontFamily: 'monospace', fontSize: '14px' }}>{inv.invoiceNumber}</td>
                                            <td>{inv.clientName}</td>
                                            <td style={{ color: '#94A3B8' }}>{inv.milestoneLabel || '-'}</td>
                                            <td>฿{inv.amount?.toLocaleString()}</td>
                                            <td style={{ fontWeight: 600 }}>฿{inv.totalAmount?.toLocaleString()}</td>
                                            <td>{inv.dueDate ? new Date(inv.dueDate).toLocaleDateString() : '-'}</td>
                                            <td>
                                                <span style={{
                                                    padding: '4px 12px',
                                                    borderRadius: '20px',
                                                    background: sc.bg,
                                                    color: sc.color,
                                                    fontSize: '12px',
                                                    fontWeight: 600,
                                                    textTransform: 'uppercase'
                                                }}>{inv.status}</span>
                                            </td>
                                            <td>
                                                <Link to={`/invoices/${inv.id}`} className="btn-secondary" style={{ padding: '6px 12px', fontSize: '13px', textDecoration: 'none' }}>
                                                    View →
                                                </Link>
                                            </td>
                                        </tr>
                                    );
                                })}
                            </tbody>
                        </table>
                    </div>

                    {/* Pagination */}
                    {totalPages > 1 && (
                        <div style={{ display: 'flex', justifyContent: 'center', gap: '8px', marginTop: '24px' }}>
                            <button className="btn-secondary" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>← Prev</button>
                            <span style={{ padding: '8px 16px', color: '#94A3B8' }}>Page {page} of {totalPages}</span>
                            <button className="btn-secondary" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>Next →</button>
                        </div>
                    )}
                </>
            )}
        </div>
    );
};

export default InvoiceList;
