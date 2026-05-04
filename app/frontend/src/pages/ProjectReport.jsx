import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../utils/api';

const ProjectReport = () => {
    const { projectId } = useParams();
    const [report, setReport] = useState(null);
    const [loading, setLoading] = useState(true);
    const [downloading, setDownloading] = useState(false);

    useEffect(() => {
        const fetchReport = async () => {
            try {
                const data = await api.get(`/reports/project/${projectId}/summary`);
                setReport(data);
            } catch (err) {
                console.error('Failed to load report', err);
            } finally {
                setLoading(false);
            }
        };
        fetchReport();
    }, [projectId]);

    const handleDownloadPdf = async () => {
        setDownloading(true);
        try {
            const response = await api.get(`/reports/project/${projectId}/summary/pdf`, { responseType: 'blob' });
            // The api interceptor returns response.data, so for blob we need raw axios
            const token = localStorage.getItem('token');
            const res = await fetch(`/api/reports/project/${projectId}/summary/pdf`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            const blob = await res.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `Report_${report?.projectName || 'Project'}.pdf`;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        } catch (err) {
            console.error('Failed to download PDF', err);
        } finally {
            setDownloading(false);
        }
    };

    if (loading) return <div className="loader-container"><div className="spinner"></div></div>;
    if (!report) return <div className="page-container"><p>Report not found.</p></div>;

    const categoryColors = {
        material_cost: { bg: 'rgba(59,130,246,0.2)', color: '#60A5FA', icon: '🧱' },
        labor_cost: { bg: 'rgba(245,158,11,0.2)', color: '#FCD34D', icon: '👷' },
        other_cost: { bg: 'rgba(168,85,247,0.2)', color: '#C084FC', icon: '📋' }
    };

    return (
        <div className="page-container fadeIn">
            <header className="top-header detail-header">
                <div>
                    <Link to={`/projects/${projectId}`} className="breadcrumb">← Back to Project</Link>
                    <h1>📊 Project Report</h1>
                    <p style={{ color: '#94A3B8', fontSize: '14px', marginTop: '4px' }}>
                        {report.projectName} • <span style={{ textTransform: 'uppercase' }}>{report.status}</span>
                    </p>
                </div>
                <button
                    className="btn-primary"
                    style={{ width: 'auto' }}
                    onClick={handleDownloadPdf}
                    disabled={downloading}
                >
                    {downloading ? '⏳ Generating...' : '📄 Download PDF'}
                </button>
            </header>

            {/* Financial KPI Cards */}
            <section className="stats-grid" style={{ marginTop: '24px' }}>
                <div className="stat-card glass-panel">
                    <h3>Budget</h3>
                    <p className="big-num" style={{ fontSize: '28px' }}>฿{report.budget?.toLocaleString()}</p>
                </div>
                <div className="stat-card glass-panel">
                    <h3>Total Expenses</h3>
                    <p className="big-num text-danger" style={{ fontSize: '28px' }}>฿{report.totalExpenses?.toLocaleString()}</p>
                </div>
                <div className="stat-card glass-panel">
                    <h3>Revenue (Paid)</h3>
                    <p className="big-num text-success" style={{ fontSize: '28px' }}>฿{report.totalPaid?.toLocaleString()}</p>
                </div>
                <div className="stat-card glass-panel">
                    <h3>Outstanding</h3>
                    <p className="big-num" style={{ fontSize: '28px', color: '#FCD34D' }}>฿{report.outstandingReceivable?.toLocaleString()}</p>
                </div>
            </section>

            <div className="detail-grid" style={{ marginTop: '24px' }}>
                {/* Main Pane: Expense Breakdown */}
                <div className="main-pane">
                    <h2 className="section-title">Expense Breakdown</h2>
                    <div className="glass-panel" style={{ padding: '24px' }}>
                        {report.expenseBreakdown?.length > 0 ? (
                            <>
                                {report.expenseBreakdown.map((item, i) => {
                                    const cc = categoryColors[item.category] || categoryColors.other_cost;
                                    return (
                                        <div key={i} style={{ marginBottom: '16px' }}>
                                            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '6px' }}>
                                                <span style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                                                    <span style={{
                                                        padding: '4px 10px',
                                                        borderRadius: '8px',
                                                        background: cc.bg,
                                                        color: cc.color,
                                                        fontSize: '13px',
                                                        fontWeight: 600
                                                    }}>
                                                        {cc.icon} {item.category.replace('_cost', '')}
                                                    </span>
                                                </span>
                                                <span style={{ fontWeight: 600 }}>
                                                    ฿{item.amount?.toLocaleString()} <span style={{ color: '#94A3B8', fontWeight: 400, fontSize: '13px' }}>({item.percentage}%)</span>
                                                </span>
                                            </div>
                                            <div className="pc-bar-bg" style={{ height: '10px' }}>
                                                <div
                                                    style={{
                                                        height: '100%',
                                                        width: `${item.percentage}%`,
                                                        background: cc.color,
                                                        borderRadius: '4px',
                                                        transition: 'width 1s ease-in-out'
                                                    }}
                                                ></div>
                                            </div>
                                        </div>
                                    );
                                })}
                                <hr className="divider" />
                                <div style={{ display: 'flex', justifyContent: 'space-between', fontWeight: 600, fontSize: '16px' }}>
                                    <span>Total Expenses</span>
                                    <span className="text-danger">฿{report.totalExpenses?.toLocaleString()}</span>
                                </div>
                            </>
                        ) : (
                            <p style={{ color: '#94A3B8', textAlign: 'center', padding: '20px' }}>No expenses recorded yet.</p>
                        )}
                    </div>
                </div>

                {/* Side Pane: Profit/Loss */}
                <div className="side-pane">
                    <h2 className="section-title">Profit / Loss</h2>
                    <div className="glass-panel summary-card">
                        <div className="summary-row">
                            <span>Revenue (Paid)</span>
                            <span className="text-success" style={{ fontWeight: 600 }}>฿{report.totalPaid?.toLocaleString()}</span>
                        </div>
                        <div className="summary-row">
                            <span>Expenses</span>
                            <span className="text-danger" style={{ fontWeight: 600 }}>- ฿{report.totalExpenses?.toLocaleString()}</span>
                        </div>
                        <hr className="divider" />
                        <div className="summary-row large">
                            <span>Profit</span>
                            <span style={{ color: report.profit >= 0 ? '#34D399' : '#FCA5A5' }}>
                                ฿{report.profit?.toLocaleString()}
                            </span>
                        </div>
                        <div style={{ 
                            marginTop: '16px', 
                            padding: '16px', 
                            borderRadius: '12px',
                            background: report.profit >= 0 ? 'rgba(16,185,129,0.1)' : 'rgba(239,68,68,0.1)',
                            textAlign: 'center'
                        }}>
                            <p style={{ fontSize: '12px', color: '#94A3B8', marginBottom: '4px' }}>PROFIT MARGIN</p>
                            <p style={{ 
                                fontSize: '32px', 
                                fontWeight: 700,
                                color: report.profit >= 0 ? '#34D399' : '#FCA5A5'
                            }}>
                                {report.profitMarginPercent}%
                            </p>
                        </div>

                        <div style={{ marginTop: '24px' }}>
                            <h3 style={{ fontSize: '13px', color: '#94A3B8', marginBottom: '12px', textTransform: 'uppercase' }}>Invoicing</h3>
                            <div className="summary-row">
                                <span>Total Invoiced</span>
                                <span style={{ fontWeight: 600 }}>฿{report.totalInvoiced?.toLocaleString()}</span>
                            </div>
                            <div className="summary-row">
                                <span>Outstanding</span>
                                <span style={{ fontWeight: 600, color: '#FCD34D' }}>฿{report.outstandingReceivable?.toLocaleString()}</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ProjectReport;
