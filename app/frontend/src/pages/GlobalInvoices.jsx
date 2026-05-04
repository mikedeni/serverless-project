import React, { useEffect, useState } from 'react';
import api from '../utils/api';
import { Link } from 'react-router-dom';
import { Receipt, ArrowRight, ShieldCheck, Clock } from 'lucide-react';

const GlobalInvoices = () => {
    const [projects, setProjects] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchProjects = async () => {
            try {
                const data = await api.get('/dashboard/summary');
                setProjects(data.activeProjectsProgress || []);
            } catch (err) {
                console.error("Error loading project invoices", err);
            } finally {
                setLoading(false);
            }
        };
        fetchProjects();
    }, []);

    return (
        <div className="page-container fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh' }}>
            <header className="top-header" style={{ marginBottom: '40px' }}>
                <div>
                    <p className="text-label-caps" style={{ marginBottom: '4px' }}>Financial Management</p>
                    <h1 className="text-h1">Project Invoices</h1>
                </div>
            </header>

            <div className="glass-panel" style={{ 
                padding: '32px', 
                marginBottom: '40px', 
                background: 'linear-gradient(135deg, var(--primary) 0%, var(--secondary) 100%)', 
                color: 'white',
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
            }}>
                <div>
                    <h2 className="text-h2" style={{ color: 'white', marginBottom: '8px' }}>Billing Dashboard</h2>
                    <p style={{ opacity: 0.9, maxWidth: '600px' }}>Centralized hub for managing project billing milestones, client payments, and tax tracking. Select a project to manage its financial records.</p>
                </div>
                <div className="hidden md:block">
                    <Receipt size={64} style={{ opacity: 0.2 }} />
                </div>
            </div>

            {loading ? (
                <div className="grid">
                    {[1,2,3].map(i => <div key={i} className="skeleton-loader" style={{ height: '240px' }}></div>)}
                </div>
            ) : (
                <div className="grid">
                    {projects.map(p => (
                        <div key={p.id} className="glass-panel interactive-card" style={{ padding: '32px', display: 'flex', flexDirection: 'column' }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '24px' }}>
                                <div style={{ background: 'rgba(15, 76, 129, 0.1)', padding: '12px', borderRadius: '12px' }}>
                                    <Receipt size={28} color="var(--primary)" />
                                </div>
                                <div style={{ textAlign: 'right' }}>
                                    <span className="status-badge" style={{ background: 'var(--surface-variant)', color: 'var(--on-surface-variant)' }}>
                                        {p.status}
                                    </span>
                                </div>
                            </div>
                            
                            <h3 className="text-h3" style={{ marginBottom: '12px' }}>{p.projectName}</h3>
                            <p style={{ color: 'var(--text-muted)', fontSize: '14px', marginBottom: '24px', flex: 1 }}>
                                Detailed tracking for client invoices and payment collection.
                            </p>

                            <div style={{ display: 'flex', gap: '16px', marginBottom: '24px' }}>
                                <div style={{ display: 'flex', alignItems: 'center', gap: '8px', fontSize: '13px', color: 'var(--success)' }}>
                                    <ShieldCheck size={16} />
                                    <span>Verified</span>
                                </div>
                                <div style={{ display: 'flex', alignItems: 'center', gap: '8px', fontSize: '13px', color: 'var(--text-muted)' }}>
                                    <Clock size={16} />
                                    <span>Updated 2h ago</span>
                                </div>
                            </div>
                            
                            <Link 
                                to={`/projects/${p.id}/invoices`} 
                                className="btn-primary" 
                                style={{ width: '100%', textDecoration: 'none' }}
                            >
                                Manage Billing <ArrowRight size={18} />
                            </Link>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default GlobalInvoices;
