import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../utils/api';
import { 
    Truck, 
    UserPlus, 
    Search, 
    Mail, 
    Phone, 
    Shield, 
    ArrowRight, 
    ChevronLeft, 
    ChevronRight,
    X,
    Check,
    Briefcase
} from 'lucide-react';

const SubcontractorList = () => {
    const [subs, setSubs] = useState([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [showForm, setShowForm] = useState(false);
    const [creating, setCreating] = useState(false);
    const [error, setError] = useState('');
    const [search, setSearch] = useState('');

    const [form, setForm] = useState({ name: '', specialty: '', phone: '', email: '' });

    const fetchSubs = async () => {
        try {
            setLoading(true);
            const params = new URLSearchParams({ page, pageSize: 10 });
            if (search) params.append('search', search);
            const data = await api.get(`/subcontractors?${params}`);
            setSubs(data.items || []);
            setTotalPages(data.totalPages || 1);
        } catch (err) {
            console.error('Failed to load subcontractors', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { fetchSubs(); }, [page, search]);

    const handleCreate = async (e) => {
        e.preventDefault();
        setCreating(true);
        setError('');
        try {
            await api.post('/subcontractors', form);
            setShowForm(false);
            setForm({ name: '', specialty: '', phone: '', email: '' });
            fetchSubs();
        } catch (err) {
            setError(err.message);
        } finally {
            setCreating(false);
        }
    };

    return (
        <div className="page-container fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh' }}>
            <header className="top-header" style={{ marginBottom: '40px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <p className="text-label-caps" style={{ marginBottom: '4px' }}>Partner Management</p>
                    <h1 className="text-h1">Subcontractors</h1>
                </div>
                <button className="btn-primary" style={{ width: 'auto' }} onClick={() => setShowForm(!showForm)}>
                    {showForm ? <><X size={20} /> Cancel</> : <><UserPlus size={20} /> Add Partner</>}
                </button>
            </header>

            {/* Search Bar */}
            <div className="glass-panel" style={{ padding: '16px', marginBottom: '32px', display: 'flex', gap: '16px' }}>
                <div style={{ position: 'relative', flex: 1 }}>
                    <Search size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                    <input 
                        className="input-field"
                        style={{ paddingLeft: '40px', marginBottom: 0 }}
                        placeholder="Search partners by name..." 
                        value={search} 
                        onChange={e => { setSearch(e.target.value); setPage(1); }} 
                    />
                </div>
            </div>

            {showForm && (
                <div className="glass-panel fadeIn" style={{ padding: '32px', marginBottom: '32px', borderLeft: '4px solid var(--secondary)' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
                        <h3 className="text-h3">Register Subcontractor</h3>
                        <button onClick={() => setShowForm(false)} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--text-muted)' }}><X size={24} /></button>
                    </div>
                    {error && <div className="auth-error" style={{ marginBottom: '24px' }}>{error}</div>}
                    <form onSubmit={handleCreate}>
                        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: '24px' }}>
                            <div className="input-group">
                                <label className="text-label-caps">Company / Individual Name *</label>
                                <input className="input-field" type="text" value={form.name} onChange={e => setForm({...form, name: e.target.value})} required placeholder="Name" />
                            </div>
                            <div className="input-group">
                                <label className="text-label-caps">Specialty / Service</label>
                                <input className="input-field" type="text" value={form.specialty} onChange={e => setForm({...form, specialty: e.target.value})} placeholder="e.g. Electrical, Plumbing" />
                            </div>
                            <div className="input-group">
                                <label className="text-label-caps">Contact Phone</label>
                                <input className="input-field" type="text" value={form.phone} onChange={e => setForm({...form, phone: e.target.value})} placeholder="08x-xxx-xxxx" />
                            </div>
                            <div className="input-group">
                                <label className="text-label-caps">Email Address</label>
                                <input className="input-field" type="email" value={form.email} onChange={e => setForm({...form, email: e.target.value})} placeholder="partner@example.com" />
                            </div>
                        </div>
                        <div style={{ display: 'flex', gap: '16px', marginTop: '32px' }}>
                            <button type="submit" className="btn-primary" style={{ width: 'auto' }} disabled={creating}>
                                {creating ? <div className="spinner-small"></div> : <><Check size={20} /> Register Partner</>}
                            </button>
                            <button type="button" onClick={() => setShowForm(false)} className="btn-secondary" style={{ width: 'auto' }}>Cancel</button>
                        </div>
                    </form>
                </div>
            )}

            {loading ? (
                <div className="grid">
                    {[1,2,3].map(i => <div key={i} className="skeleton-loader" style={{ height: '100px' }}></div>)}
                </div>
            ) : subs.length === 0 ? (
                <div className="glass-panel" style={{ padding: '80px', textAlign: 'center' }}>
                    <Truck size={48} color="var(--outline-variant)" style={{ marginBottom: '16px' }} />
                    <h2 className="text-h3" style={{ color: 'var(--text-muted)' }}>No Subcontractors Found</h2>
                    <p style={{ color: 'var(--text-muted)' }}>Connect with external partners to manage project tasks.</p>
                </div>
            ) : (
                <>
                    <div className="glass-panel table-wrapper">
                        <table className="modern-table">
                            <thead>
                                <tr>
                                    <th>Partner Details</th>
                                    <th>Specialty</th>
                                    <th>Contact</th>
                                    <th>Status</th>
                                    <th style={{ textAlign: 'right' }}>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {subs.map(s => (
                                    <tr key={s.id}>
                                        <td>
                                            <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                                <div style={{ width: '36px', height: '36px', background: 'var(--primary-container)', borderRadius: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--primary)', fontWeight: 700 }}>
                                                    {s.name.charAt(0)}
                                                </div>
                                                <span style={{ fontWeight: 600 }}>{s.name}</span>
                                            </div>
                                        </td>
                                        <td>
                                            <div style={{ display: 'flex', alignItems: 'center', gap: '8px', color: 'var(--on-surface-variant)' }}>
                                                <Briefcase size={14} /> {s.specialty || '-'}
                                            </div>
                                        </td>
                                        <td>
                                            <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
                                                <div style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '13px', color: 'var(--on-surface)' }}>
                                                    <Phone size={12} color="var(--text-muted)" /> {s.phone || '-'}
                                                </div>
                                                <div style={{ display: 'flex', alignItems: 'center', gap: '6px', fontSize: '12px', color: 'var(--text-muted)' }}>
                                                    <Mail size={12} /> {s.email || '-'}
                                                </div>
                                            </div>
                                        </td>
                                        <td>
                                            <span className="status-badge" style={{
                                                background: s.status === 'active' ? 'rgba(16, 185, 129, 0.1)' : 'rgba(107, 114, 128, 0.1)',
                                                color: s.status === 'active' ? 'var(--success)' : 'var(--text-muted)'
                                            }}>
                                                {s.status}
                                            </span>
                                        </td>
                                        <td>
                                            <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
                                                <Link to={`/subcontractors/${s.id}`} className="btn-secondary" style={{ padding: '8px 16px', fontSize: '13px', textDecoration: 'none' }}>
                                                    Profile <ArrowRight size={14} />
                                                </Link>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                    {totalPages > 1 && (
                        <div style={{ display: 'flex', justifyContent: 'center', gap: '12px', marginTop: '32px' }}>
                            <button className="btn-secondary" disabled={page <= 1} onClick={() => setPage(p => p - 1)} style={{ width: 'auto' }}><ChevronLeft size={18} /> Prev</button>
                            <span style={{ display: 'flex', alignItems: 'center', padding: '0 16px', color: 'var(--text-muted)', fontWeight: 600 }}>Page {page} of {totalPages}</span>
                            <button className="btn-secondary" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)} style={{ width: 'auto' }}>Next <ChevronRight size={18} /></button>
                        </div>
                    )}
                </>
            )}
        </div>
    );
};

export default SubcontractorList;
