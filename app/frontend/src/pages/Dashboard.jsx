import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../contexts/AuthContext';
import api from '../utils/api';
import ProjectSummaryCard from '../components/ProjectSummaryCard';
import { Link } from 'react-router-dom';
import { 
    Plus, 
    LayoutDashboard, 
    Wallet, 
    ArrowUpRight, 
    ArrowDownLeft, 
    CheckCircle2,
    Briefcase
} from 'lucide-react';

const Dashboard = () => {
    const { user, logout } = useContext(AuthContext);
    const [metrics, setMetrics] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchDashboard = async () => {
            try {
                const data = await api.get('/dashboard/summary');
                setMetrics(data);
            } catch (err) {
                console.error("Failed to load dashboard metrics", err);
            } finally {
                setLoading(false);
            }
        };

        fetchDashboard();
    }, []);

    return (
        <div className="dashboard-content fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh' }}>
            <header className="top-header" style={{ marginBottom: '40px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <p className="text-label-caps" style={{ marginBottom: '4px' }}>Overview</p>
                    <h1 className="text-h1">แดชบอร์ด</h1>
                </div>
                <Link to="/projects/new" className="btn-primary" style={{ textDecoration: 'none' }}>
                    <Plus size={20} /> New Project
                </Link>
            </header>
            
            {loading ? (
                <div className="grid" style={{ gap: '24px' }}>
                    {[1,2,3,4,5].map(i => <div key={i} className="skeleton-loader" style={{ height: '140px' }}></div>)}
                </div>
            ) : (
                <>
                    <section className="stats-grid" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: '24px', marginBottom: '40px' }}>
                        {/* Total Projects */}
                        <div className="glass-panel" style={{ padding: '24px' }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '16px' }}>
                                <div style={{ background: 'rgba(15, 76, 129, 0.1)', padding: '12px', borderRadius: '12px' }}>
                                    <Briefcase size={24} color="var(--primary)" />
                                </div>
                            </div>
                            <p className="text-label-caps">Total Projects</p>
                            <p className="text-h2" style={{ marginTop: '8px' }}>{metrics?.totalProjects || 0}</p>
                        </div>

                        {/* Global Budget */}
                        <div className="glass-panel" style={{ padding: '24px' }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '16px' }}>
                                <div style={{ background: 'rgba(15, 76, 129, 0.1)', padding: '12px', borderRadius: '12px' }}>
                                    <Wallet size={24} color="var(--primary)" />
                                </div>
                            </div>
                            <p className="text-label-caps">Global Master Budget</p>
                            <p className="text-h2" style={{ marginTop: '8px', color: 'var(--primary)' }}>฿{(metrics?.totalBudget || 0).toLocaleString()}</p>
                        </div>

                        {/* Global Expenses */}
                        <div className="glass-panel" style={{ padding: '24px' }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '16px' }}>
                                <div style={{ background: 'rgba(186, 26, 26, 0.1)', padding: '12px', borderRadius: '12px' }}>
                                    <ArrowDownLeft size={24} color="var(--error)" />
                                </div>
                                <span className="status-badge" style={{ background: 'var(--error-container)', color: 'var(--on-error-container)' }}>
                                    {metrics?.budgetVsActualPercentage || 0}% Used
                                </span>
                            </div>
                            <p className="text-label-caps">Global Expenses</p>
                            <p className="text-h2" style={{ marginTop: '8px', color: 'var(--error)' }}>฿{(metrics?.totalExpenses || 0).toLocaleString()}</p>
                        </div>

                        {/* Receivables */}
                        <div className="glass-panel" style={{ padding: '24px', borderTop: '4px solid var(--success)' }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '16px' }}>
                                <div style={{ background: 'rgba(16, 185, 129, 0.1)', padding: '12px', borderRadius: '12px' }}>
                                    <ArrowUpRight size={24} color="var(--success)" />
                                </div>
                            </div>
                            <p className="text-label-caps">Receivables (รอรับ)</p>
                            <p className="text-h2" style={{ marginTop: '8px', color: 'var(--success)' }}>฿{(metrics?.totalReceivables || 0).toLocaleString()}</p>
                        </div>

                        {/* Payables */}
                        <div className="glass-panel" style={{ padding: '24px', borderTop: '4px solid var(--warning)' }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '16px' }}>
                                <div style={{ background: 'rgba(245, 158, 11, 0.1)', padding: '12px', borderRadius: '12px' }}>
                                    <ArrowDownLeft size={24} color="var(--warning)" />
                                </div>
                            </div>
                            <p className="text-label-caps">Payables (รอจ่าย Sub)</p>
                            <p className="text-h2" style={{ marginTop: '8px', color: 'var(--warning)' }}>฿{(metrics?.totalPayables || 0).toLocaleString()}</p>
                        </div>
                    </section>

                    <section className="project-list">
                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
                            <h2 className="text-h3">Active Projects</h2>
                            <Link to="/projects" className="text-body-sm" style={{ color: 'var(--secondary)', fontWeight: 600 }}>View All Projects →</Link>
                        </div>
                        <div className="grid">
                            {metrics?.activeProjectsProgress?.length > 0 ? (
                                metrics.activeProjectsProgress.map(proj => (
                                    <ProjectSummaryCard key={proj.id} project={proj} />
                                ))
                            ) : (
                                <div className="glass-panel" style={{ padding: '40px', textAlign: 'center', color: 'var(--text-muted)' }}>
                                    No projects found. Create one to get started!
                                </div>
                            )}
                        </div>
                    </section>
                </>
            )}
        </div>
    );
};

export default Dashboard;
