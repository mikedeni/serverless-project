import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../utils/api';

const typeLabels = {
    purchase_in: { label: 'Purchase In', color: '#34D399', bg: 'rgba(16,185,129,0.2)', sign: '+' },
    requisition_out: { label: 'Requisition Out', color: '#FCA5A5', bg: 'rgba(239,68,68,0.2)', sign: '-' },
    return: { label: 'Return', color: '#60A5FA', bg: 'rgba(59,130,246,0.2)', sign: '+' },
    adjustment: { label: 'Adjustment', color: '#FCD34D', bg: 'rgba(245,158,11,0.2)', sign: '=' }
};

const MaterialDetail = () => {
    const { id } = useParams();
    const [detail, setDetail] = useState(null);
    const [loading, setLoading] = useState(true);
    const [showForm, setShowForm] = useState(false);
    const [projects, setProjects] = useState([]);
    const [formData, setFormData] = useState({
        materialId: parseInt(id),
        projectId: '',
        type: 'purchase_in',
        qty: 0,
        unitPrice: 0,
        note: '',
        date: new Date().toISOString().split('T')[0]
    });

    useEffect(() => {
        fetchDetail();
        fetchProjects();
    }, [id]);

    const fetchDetail = async () => {
        try {
            const data = await api.get(`/materials/${id}`);
            setDetail(data);
        } catch (err) {
            console.error('Failed to load material', err);
        } finally {
            setLoading(false);
        }
    };

    const fetchProjects = async () => {
        try {
            const data = await api.get('/projects?pageSize=50');
            setProjects(data.items || []);
        } catch (err) {
            console.error('Failed to load projects', err);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const payload = {
                ...formData,
                materialId: parseInt(id),
                projectId: formData.projectId ? parseInt(formData.projectId) : null
            };
            await api.post('/materials/transactions', payload);
            setShowForm(false);
            setFormData({ materialId: parseInt(id), projectId: '', type: 'purchase_in', qty: 0, unitPrice: 0, note: '', date: new Date().toISOString().split('T')[0] });
            fetchDetail();
        } catch (err) {
            alert(err.message);
        }
    };

    const inputStyle = {
        width: '100%', padding: '12px', background: 'rgba(0,0,0,0.2)',
        border: '1px solid rgba(255,255,255,0.1)', borderRadius: '8px',
        color: '#fff', fontSize: '14px', outline: 'none'
    };
    const labelStyle = { display: 'block', marginBottom: '6px', fontSize: '12px', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.5px' };

    if (loading) return <div className="loader-container"><div className="spinner"></div></div>;
    if (!detail) return <div className="page-container">Material not found.</div>;

    const { material, recentTransactions } = detail;
    const isLow = material.minStock > 0 && material.currentStock <= material.minStock;

    return (
        <div className="page-container fadeIn">
            <header className="top-header">
                <div>
                    <Link to="/materials" className="breadcrumb">← Back to Materials</Link>
                    <h1>{material.name}</h1>
                </div>
                <button onClick={() => setShowForm(!showForm)} className="btn-primary" style={{ width: 'auto' }}>
                    {showForm ? 'Cancel' : '+ New Transaction'}
                </button>
            </header>

            {/* Stats */}
            <div className="stats-grid" style={{ marginBottom: '24px' }}>
                <div className="stat-card glass-panel">
                    <h3>Current Stock</h3>
                    <p className={`big-num ${isLow ? 'text-danger' : ''}`}>{material.currentStock.toLocaleString()} <span style={{ fontSize: '16px', color: 'var(--text-muted)' }}>{material.unit}</span></p>
                    {isLow && <span style={{ fontSize: '12px', color: '#FCD34D' }}>⚠️ Below minimum stock ({material.minStock})</span>}
                </div>
                <div className="stat-card glass-panel">
                    <h3>Min Stock Alert</h3>
                    <p className="big-num">{material.minStock} <span style={{ fontSize: '16px', color: 'var(--text-muted)' }}>{material.unit}</span></p>
                </div>
                <div className="stat-card glass-panel">
                    <h3>Last Price</h3>
                    <p className="big-num highlight">{material.lastPrice > 0 ? `฿${material.lastPrice.toLocaleString()}` : '-'}</p>
                </div>
            </div>

            {/* Transaction Form */}
            {showForm && (
                <div className="glass-panel" style={{ padding: '24px', marginBottom: '24px' }}>
                    <h3 style={{ marginBottom: '20px' }}>Record Transaction</h3>
                    <form onSubmit={handleSubmit}>
                        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: '16px' }}>
                            <div>
                                <label style={labelStyle}>Type *</label>
                                <select style={inputStyle} value={formData.type} onChange={(e) => setFormData({ ...formData, type: e.target.value })}>
                                    <option value="purchase_in" style={{ background: '#1e293b' }}>📦 Purchase In</option>
                                    <option value="requisition_out" style={{ background: '#1e293b' }}>📤 Requisition Out</option>
                                    <option value="return" style={{ background: '#1e293b' }}>🔄 Return</option>
                                    <option value="adjustment" style={{ background: '#1e293b' }}>📝 Adjustment</option>
                                </select>
                            </div>
                            <div>
                                <label style={labelStyle}>Quantity *</label>
                                <input style={inputStyle} type="number" min="0" step="0.01" required value={formData.qty} onChange={(e) => setFormData({ ...formData, qty: parseFloat(e.target.value) || 0 })} />
                            </div>
                            <div>
                                <label style={labelStyle}>Unit Price (฿)</label>
                                <input style={inputStyle} type="number" min="0" step="0.01" value={formData.unitPrice} onChange={(e) => setFormData({ ...formData, unitPrice: parseFloat(e.target.value) || 0 })} />
                            </div>
                            <div>
                                <label style={labelStyle}>Project (optional)</label>
                                <select style={inputStyle} value={formData.projectId} onChange={(e) => setFormData({ ...formData, projectId: e.target.value })}>
                                    <option value="" style={{ background: '#1e293b' }}>-- None --</option>
                                    {projects.map(p => <option key={p.id} value={p.id} style={{ background: '#1e293b' }}>{p.projectName}</option>)}
                                </select>
                            </div>
                            <div>
                                <label style={labelStyle}>Date *</label>
                                <input style={inputStyle} type="date" required value={formData.date} onChange={(e) => setFormData({ ...formData, date: e.target.value })} />
                            </div>
                            <div>
                                <label style={labelStyle}>Note</label>
                                <input style={inputStyle} value={formData.note} onChange={(e) => setFormData({ ...formData, note: e.target.value })} placeholder="หมายเหตุ" />
                            </div>
                        </div>
                        <button type="submit" className="btn-primary" style={{ width: 'auto', marginTop: '20px' }}>Record Transaction</button>
                    </form>
                </div>
            )}

            {/* Transaction History */}
            <h2 className="section-title">Transaction History</h2>
            <div className="glass-panel table-wrapper">
                {recentTransactions.length === 0 ? (
                    <p style={{ padding: '24px', textAlign: 'center', color: 'var(--text-muted)' }}>No transactions yet</p>
                ) : (
                    <table className="modern-table">
                        <thead>
                            <tr>
                                <th>Date</th>
                                <th>Type</th>
                                <th>Qty</th>
                                <th>Unit Price</th>
                                <th>Total</th>
                                <th>Project</th>
                                <th>Note</th>
                            </tr>
                        </thead>
                        <tbody>
                            {recentTransactions.map(t => {
                                const tl = typeLabels[t.type] || typeLabels.adjustment;
                                return (
                                    <tr key={t.id}>
                                        <td>{new Date(t.date).toLocaleDateString()}</td>
                                        <td>
                                            <span style={{ padding: '4px 10px', borderRadius: '6px', fontSize: '11px', fontWeight: 600, background: tl.bg, color: tl.color }}>
                                                {tl.label}
                                            </span>
                                        </td>
                                        <td style={{ fontWeight: 600, color: tl.color }}>{tl.sign}{t.qty} {material.unit}</td>
                                        <td>{t.unitPrice > 0 ? `฿${t.unitPrice.toLocaleString()}` : '-'}</td>
                                        <td style={{ fontWeight: 600 }}>{t.unitPrice > 0 ? `฿${(t.qty * t.unitPrice).toLocaleString()}` : '-'}</td>
                                        <td style={{ color: 'var(--text-muted)' }}>{t.projectName || '-'}</td>
                                        <td style={{ color: 'var(--text-muted)', fontSize: '13px' }}>{t.note || '-'}</td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    );
};

export default MaterialDetail;
