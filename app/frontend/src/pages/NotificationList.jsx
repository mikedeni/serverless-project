import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api from '../utils/api';
import { 
    Bell, 
    Clock, 
    Wallet, 
    Package, 
    FileText, 
    CheckCheck, 
    ChevronLeft, 
    ChevronRight,
    ArrowLeft,
    Inbox
} from 'lucide-react';

const typeIcons = {
    task_overdue: <Clock size={20} color="var(--error)" />,
    budget_warning: <Wallet size={20} color="var(--warning)" />,
    low_stock: <Package size={20} color="var(--warning)" />,
    invoice_overdue: <FileText size={20} color="var(--error)" />,
    general: <Bell size={20} color="var(--primary)" />
};

const NotificationList = () => {
    const [notifications, setNotifications] = useState([]);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const navigate = useNavigate();

    const fetchNotifications = async () => {
        try {
            setLoading(true);
            const data = await api.get(`/notifications?page=${page}&pageSize=20`);
            setNotifications(data.items || []);
            setTotalPages(data.totalPages || 1);
        } catch (err) {
            console.error('Failed to load notifications', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { fetchNotifications(); }, [page]);

    const handleClick = async (n) => {
        if (!n.isRead) {
            await api.put(`/notifications/${n.id}/read`);
        }
        if (n.relatedUrl) {
            navigate(n.relatedUrl);
        }
        fetchNotifications();
    };

    const handleMarkAllRead = async () => {
        await api.put('/notifications/mark-all-read');
        fetchNotifications();
    };

    return (
        <div className="page-container fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh' }}>
            <header className="top-header" style={{ marginBottom: '40px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <p className="text-label-caps" style={{ marginBottom: '4px' }}>System Logs</p>
                    <h1 className="text-h1">Notifications</h1>
                </div>
                <div style={{ display: 'flex', gap: '12px' }}>
                    <button className="btn-secondary" style={{ width: 'auto' }} onClick={handleMarkAllRead}>
                        <CheckCheck size={18} /> Mark All Read
                    </button>
                    <Link to="/dashboard" className="btn-secondary" style={{ width: 'auto', textDecoration: 'none' }}>
                        <ArrowLeft size={18} /> Back
                    </Link>
                </div>
            </header>

            {loading ? (
                <div style={{ display: 'grid', gap: '12px' }}>
                    {[1,2,3,4,5].map(i => <div key={i} className="skeleton-loader" style={{ height: '80px' }}></div>)}
                </div>
            ) : notifications.length === 0 ? (
                <div className="glass-panel" style={{ padding: '80px', textAlign: 'center' }}>
                    <Inbox size={48} color="var(--outline-variant)" style={{ marginBottom: '16px' }} />
                    <h2 className="text-h3" style={{ color: 'var(--text-muted)' }}>You're all caught up!</h2>
                    <p style={{ color: 'var(--text-muted)' }}>No new notifications to show right now.</p>
                </div>
            ) : (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                    {notifications.map(n => (
                        <div key={n.id} className="glass-panel interactive-card fadeIn" onClick={() => handleClick(n)}
                            style={{
                                padding: '20px 24px', cursor: 'pointer', display: 'flex', gap: '20px', alignItems: 'center',
                                borderLeft: n.isRead ? '4px solid transparent' : '4px solid var(--secondary)',
                                opacity: n.isRead ? 0.7 : 1,
                                background: n.isRead ? 'var(--surface)' : 'rgba(37, 99, 235, 0.02)'
                            }}
                        >
                            <div style={{ 
                                width: '48px', 
                                height: '48px', 
                                borderRadius: '12px', 
                                background: n.isRead ? 'var(--surface-variant)' : 'var(--secondary-container)', 
                                display: 'flex', 
                                alignItems: 'center', 
                                justifyContent: 'center' 
                            }}>
                                {typeIcons[n.type] || <Bell size={20} color="var(--primary)" />}
                            </div>
                            <div style={{ flex: 1 }}>
                                <p style={{ fontWeight: n.isRead ? 500 : 700, fontSize: '16px', color: 'var(--on-surface)', marginBottom: '4px' }}>{n.title}</p>
                                <p style={{ color: 'var(--on-surface-variant)', fontSize: '14px' }}>{n.message}</p>
                            </div>
                            <div style={{ textAlign: 'right' }}>
                                <p style={{ color: 'var(--text-muted)', fontSize: '12px', marginBottom: '8px' }}>
                                    {new Date(n.createdAt).toLocaleDateString()} {new Date(n.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                </p>
                                {!n.isRead && <div style={{ display: 'inline-block', width: '8px', height: '8px', borderRadius: '50%', background: 'var(--secondary)' }}></div>}
                            </div>
                        </div>
                    ))}
                    {totalPages > 1 && (
                        <div style={{ display: 'flex', justifyContent: 'center', gap: '12px', marginTop: '32px' }}>
                            <button className="btn-secondary" disabled={page <= 1} onClick={() => setPage(p => p - 1)} style={{ width: 'auto' }}>
                                <ChevronLeft size={18} /> Prev
                            </button>
                            <span style={{ display: 'flex', alignItems: 'center', padding: '0 16px', color: 'var(--text-muted)', fontWeight: 600 }}>
                                Page {page} of {totalPages}
                            </span>
                            <button className="btn-secondary" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)} style={{ width: 'auto' }}>
                                Next <ChevronRight size={18} />
                            </button>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default NotificationList;
