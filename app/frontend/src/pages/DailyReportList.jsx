import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../utils/api';

const weatherIcons = { sunny: '☀️', cloudy: '⛅', rainy: '🌧️', stormy: '⛈️' };

const DailyReportList = () => {
    const { projectId } = useParams();
    const [reports, setReports] = useState([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [showForm, setShowForm] = useState(false);
    const [creating, setCreating] = useState(false);
    const [error, setError] = useState('');

    const [form, setForm] = useState({
        projectId: parseInt(projectId),
        reportDate: new Date().toISOString().split('T')[0],
        weather: 'sunny',
        workerCount: 0,
        summary: '',
        issues: '',
        photos: []
    });

    const fetchReports = async () => {
        try {
            setLoading(true);
            const data = await api.get(`/daily-reports/project/${projectId}?page=${page}&pageSize=10`);
            setReports(data.items || []);
            setTotalPages(data.totalPages || 1);
        } catch (err) {
            console.error('Failed to load reports', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { fetchReports(); }, [projectId, page]);

    const handleCreate = async (e) => {
        e.preventDefault();
        setCreating(true);
        setError('');
        try {
            await api.post('/daily-reports', { ...form, workerCount: parseInt(form.workerCount) });
            setShowForm(false);
            setForm({ projectId: parseInt(projectId), reportDate: new Date().toISOString().split('T')[0], weather: 'sunny', workerCount: 0, summary: '', issues: '', photos: [] });
            fetchReports();
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
                    <h1>📝 Daily Reports</h1>
                </div>
                <button className="btn-primary" style={{ width: 'auto' }} onClick={() => setShowForm(!showForm)}>
                    {showForm ? '✕ Cancel' : '+ New Report'}
                </button>
            </header>

            {showForm && (
                <div className="glass-panel" style={{ padding: '24px', marginBottom: '24px', animation: 'fadeIn 0.3s ease' }}>
                    <h3 style={{ marginBottom: '16px' }}>Create Daily Report</h3>
                    {error && <div className="auth-error">{error}</div>}
                    <form onSubmit={handleCreate}>
                        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                            <div className="input-group">
                                <input type="date" placeholder=" " value={form.reportDate}
                                    onChange={e => setForm({...form, reportDate: e.target.value})} required />
                                <label className="active-label">Report Date</label>
                            </div>
                            <div>
                                <label style={{ color: '#94A3B8', fontSize: '12px', marginBottom: '6px', display: 'block' }}>Weather</label>
                                <div style={{ display: 'flex', gap: '8px' }}>
                                    {Object.entries(weatherIcons).map(([key, icon]) => (
                                        <button key={key} type="button"
                                            onClick={() => setForm({...form, weather: key})}
                                            style={{
                                                padding: '10px 16px', borderRadius: '8px', fontSize: '16px', cursor: 'pointer',
                                                border: form.weather === key ? '2px solid #4F46E5' : '1px solid rgba(255,255,255,0.1)',
                                                background: form.weather === key ? 'rgba(79,70,229,0.2)' : 'rgba(255,255,255,0.05)',
                                            }}
                                        >{icon}</button>
                                    ))}
                                </div>
                            </div>
                            <div className="input-group">
                                <input type="number" placeholder=" " value={form.workerCount}
                                    onChange={e => setForm({...form, workerCount: e.target.value})} required />
                                <label>Worker Count</label>
                            </div>
                            <div className="input-group" style={{ gridColumn: '1 / -1' }}>
                                <input type="text" placeholder=" " value={form.summary}
                                    onChange={e => setForm({...form, summary: e.target.value})} required />
                                <label>Summary</label>
                            </div>
                            <div className="input-group" style={{ gridColumn: '1 / -1' }}>
                                <input type="text" placeholder=" " value={form.issues}
                                    onChange={e => setForm({...form, issues: e.target.value})} />
                                <label>Issues (if any)</label>
                            </div>
                        </div>
                        <button type="submit" className="btn-primary" style={{ width: 'auto', marginTop: '8px' }} disabled={creating}>
                            {creating ? 'Creating...' : 'Submit Report'}
                        </button>
                    </form>
                </div>
            )}

            {loading ? (
                <div className="loader-container" style={{ height: '200px' }}><div className="spinner"></div></div>
            ) : reports.length === 0 ? (
                <div className="glass-panel" style={{ padding: '40px', textAlign: 'center', color: '#94A3B8' }}>
                    No daily reports found. Start documenting progress!
                </div>
            ) : (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                    {reports.map(r => (
                        <Link to={`/daily-reports/${r.id}`} key={r.id} style={{ textDecoration: 'none', color: 'inherit' }}>
                            <div className="glass-panel interactive-card" style={{ padding: '20px', display: 'flex', alignItems: 'center', gap: '16px' }}>
                                <div style={{ fontSize: '32px' }}>{weatherIcons[r.weather] || '☀️'}</div>
                                <div style={{ flex: 1 }}>
                                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '4px' }}>
                                        <span style={{ fontWeight: 600, fontSize: '16px' }}>{new Date(r.reportDate).toLocaleDateString('th-TH', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}</span>
                                        <span style={{ padding: '4px 10px', borderRadius: '20px', background: 'rgba(59,130,246,0.2)', color: '#60A5FA', fontSize: '12px' }}>
                                            👷 {r.workerCount} workers
                                        </span>
                                    </div>
                                    <p style={{ color: '#94A3B8', fontSize: '14px', marginTop: '4px' }}>{r.summary?.substring(0, 100)}{r.summary?.length > 100 ? '...' : ''}</p>
                                    {r.issues && <p style={{ color: '#FCA5A5', fontSize: '13px', marginTop: '4px' }}>⚠️ {r.issues.substring(0, 60)}</p>}
                                </div>
                                <span style={{ color: '#94A3B8' }}>→</span>
                            </div>
                        </Link>
                    ))}

                    {totalPages > 1 && (
                        <div style={{ display: 'flex', justifyContent: 'center', gap: '8px', marginTop: '16px' }}>
                            <button className="btn-secondary" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>← Prev</button>
                            <span style={{ padding: '8px 16px', color: '#94A3B8' }}>Page {page} of {totalPages}</span>
                            <button className="btn-secondary" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>Next →</button>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default DailyReportList;
