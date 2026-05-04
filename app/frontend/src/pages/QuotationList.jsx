import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../utils/api';

const statusColors = {
    draft: { bg: 'rgba(148, 163, 184, 0.2)', color: '#94A3B8' },
    sent: { bg: 'rgba(59, 130, 246, 0.2)', color: '#60A5FA' },
    approved: { bg: 'rgba(16, 185, 129, 0.2)', color: '#34D399' },
    rejected: { bg: 'rgba(239, 68, 68, 0.2)', color: '#FCA5A5' }
};

const QuotationList = () => {
    const { projectId } = useParams();
    const [quotations, setQuotations] = useState([]);
    const [loading, setLoading] = useState(true);
    const [filterStatus, setFilterStatus] = useState('');

    useEffect(() => {
        fetchQuotations();
    }, [projectId, filterStatus]);

    const fetchQuotations = async () => {
        try {
            setLoading(true);
            const params = new URLSearchParams();
            if (filterStatus) params.append('status', filterStatus);
            const data = await api.get(`/quotations/project/${projectId}?${params.toString()}`);
            setQuotations(data.items || []);
        } catch (err) {
            console.error('Failed to load quotations', err);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!confirm('ลบใบเสนอราคานี้?')) return;
        try {
            await api.delete(`/quotations/${id}`);
            fetchQuotations();
        } catch (err) {
            alert(err.message);
        }
    };

    if (loading) return <div className="loader-container"><div className="spinner"></div></div>;

    return (
        <div className="page-container fadeIn">
            <header className="top-header">
                <div>
                    <Link to={`/projects/${projectId}`} className="breadcrumb">← Back to Project</Link>
                    <h1>Quotations / BOQ</h1>
                </div>
                <Link to={`/projects/${projectId}/quotations/new`} className="btn-primary" style={{ textDecoration: 'none', display: 'flex', alignItems: 'center', justifyContent: 'center', width: 'auto' }}>
                    + New Quotation
                </Link>
            </header>

            <div className="filter-bar" style={{ marginBottom: '24px', display: 'flex', gap: '8px' }}>
                {['', 'draft', 'sent', 'approved', 'rejected'].map(s => (
                    <button
                        key={s}
                        onClick={() => setFilterStatus(s)}
                        className={`btn-secondary ${filterStatus === s ? 'filter-active' : ''}`}
                        style={filterStatus === s ? { background: 'rgba(79,70,229,0.3)', borderColor: 'var(--primary)' } : {}}
                    >
                        {s || 'All'}
                    </button>
                ))}
            </div>

            {quotations.length === 0 ? (
                <div className="glass-panel" style={{ padding: '48px', textAlign: 'center', color: 'var(--text-muted)' }}>
                    <p style={{ fontSize: '18px', marginBottom: '8px' }}>No quotations found</p>
                    <p>Create your first quotation to get started</p>
                </div>
            ) : (
                <div className="grid">
                    {quotations.map(q => {
                        const sc = statusColors[q.status] || statusColors.draft;
                        return (
                            <Link to={`/quotations/${q.id}`} key={q.id} className="glass-panel project-card" style={{ textDecoration: 'none', color: 'inherit' }}>
                                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '12px' }}>
                                    <div>
                                        <div style={{ fontSize: '12px', color: 'var(--text-muted)', marginBottom: '4px', fontFamily: 'monospace' }}>{q.quotationNumber}</div>
                                        <div className="pc-title">{q.clientName}</div>
                                    </div>
                                    <span style={{ padding: '4px 10px', borderRadius: '6px', fontSize: '11px', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.5px', background: sc.bg, color: sc.color }}>
                                        {q.status}
                                    </span>
                                </div>
                                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '13px', color: 'var(--text-muted)' }}>
                                    <span>{q.validUntil ? `Valid until ${new Date(q.validUntil).toLocaleDateString()}` : ''}</span>
                                    <span>{new Date(q.createdAt).toLocaleDateString()}</span>
                                </div>
                                <div style={{ marginTop: '12px', textAlign: 'right' }}>
                                    <button onClick={(e) => { e.preventDefault(); e.stopPropagation(); handleDelete(q.id); }} className="btn-secondary" style={{ fontSize: '12px', padding: '4px 12px', color: '#FCA5A5', borderColor: 'rgba(239,68,68,0.3)' }}>
                                        Delete
                                    </button>
                                </div>
                            </Link>
                        );
                    })}
                </div>
            )}
        </div>
    );
};

export default QuotationList;
