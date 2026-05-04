import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../utils/api';
import { 
    Calendar, 
    Clock, 
    UserCheck, 
    UserX, 
    Save, 
    FileSpreadsheet, 
    ArrowLeft,
    CheckSquare,
    Square
} from 'lucide-react';

const AttendanceRecord = () => {
    const [projects, setProjects] = useState([]);
    const [workers, setWorkers] = useState([]);
    const [selectedProject, setSelectedProject] = useState('');
    const [selectedDate, setSelectedDate] = useState(new Date().toISOString().split('T')[0]);
    const [attendances, setAttendances] = useState([]);
    const [loading, setLoading] = useState(false);
    const [saving, setSaving] = useState(false);
    const [entries, setEntries] = useState([]);

    useEffect(() => {
        fetchProjects();
        fetchWorkers();
    }, []);

    useEffect(() => {
        if (selectedProject && selectedDate) {
            fetchAttendances();
        }
    }, [selectedProject, selectedDate, workers]);

    const fetchProjects = async () => {
        try {
            const data = await api.get('/projects?pageSize=50');
            setProjects(data.items || []);
        } catch (err) {
            console.error('Failed to load projects', err);
        }
    };

    const fetchWorkers = async () => {
        try {
            const data = await api.get('/workers/all');
            setWorkers(data || []);
        } catch (err) {
            console.error('Failed to load workers', err);
        }
    };

    const fetchAttendances = async () => {
        if (!selectedProject || workers.length === 0) return;
        setLoading(true);
        try {
            const data = await api.get(`/attendances/project/${selectedProject}?date=${selectedDate}`);
            setAttendances(data || []);

            const recordedWorkerIds = new Set((data || []).map(a => a.workerId));
            const activeWorkers = workers.filter(w => w.status === 'active');
            const newEntries = activeWorkers
                .filter(w => !recordedWorkerIds.has(w.id))
                .map(w => ({
                    workerId: w.id,
                    workerName: w.name,
                    dailyWage: w.dailyWage,
                    position: w.position,
                    checkIn: '08:00',
                    checkOut: '17:00',
                    otHours: 0,
                    note: '',
                    selected: false
                }));
            setEntries(newEntries);
        } catch (err) {
            console.error('Failed to load attendances', err);
        } finally {
            setLoading(false);
        }
    };

    const toggleEntry = (index) => {
        const updated = [...entries];
        updated[index].selected = !updated[index].selected;
        setEntries(updated);
    };

    const toggleAll = () => {
        const allSelected = entries.every(e => e.selected);
        setEntries(entries.map(e => ({ ...e, selected: !allSelected })));
    };

    const updateEntry = (index, field, value) => {
        const updated = [...entries];
        updated[index] = { ...updated[index], [field]: value };
        setEntries(updated);
    };

    const handleBulkSubmit = async () => {
        const selected = entries.filter(e => e.selected);
        if (selected.length === 0) return alert('เลือกแรงงานอย่างน้อย 1 คน');

        setSaving(true);
        try {
            const payload = {
                projectId: parseInt(selectedProject),
                date: selectedDate,
                entries: selected.map(e => ({
                    workerId: e.workerId,
                    checkIn: e.checkIn,
                    checkOut: e.checkOut,
                    otHours: e.otHours,
                    note: e.note
                }))
            };
            await api.post('/attendances/bulk', payload);
            fetchAttendances();
        } catch (err) {
            alert(err.message);
        } finally {
            setSaving(false);
        }
    };

    const totalRecordedWage = attendances.reduce((sum, a) => sum + a.dailyWage, 0);
    const totalSelectedWage = entries.filter(e => e.selected).reduce((sum, e) => sum + e.dailyWage, 0);

    return (
        <div className="page-container fadeIn blueprint-bg" style={{ padding: '40px', minHeight: '100vh' }}>
            <header className="top-header" style={{ marginBottom: '40px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <p className="text-label-caps" style={{ marginBottom: '4px' }}>Workforce Tracking</p>
                    <h1 className="text-h1">Attendance Record</h1>
                </div>
                <Link to="/dashboard" className="btn-secondary" style={{ width: 'auto' }}>
                    <ArrowLeft size={18} /> Back
                </Link>
            </header>

            {/* Controls */}
            <div className="glass-panel" style={{ padding: '24px', marginBottom: '32px', display: 'flex', gap: '24px', alignItems: 'flex-end', flexWrap: 'wrap' }}>
                <div style={{ flex: 1, minWidth: '250px' }}>
                    <label className="text-label-caps" style={{ marginBottom: '8px', display: 'block' }}>Project</label>
                    <div style={{ position: 'relative' }}>
                        <FileSpreadsheet size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                        <select 
                            className="input-field" 
                            style={{ paddingLeft: '40px', marginBottom: 0 }} 
                            value={selectedProject} 
                            onChange={(e) => setSelectedProject(e.target.value)}
                        >
                            <option value="">-- Select Project --</option>
                            {projects.map(p => <option key={p.id} value={p.id}>{p.projectName}</option>)}
                        </select>
                    </div>
                </div>
                <div>
                    <label className="text-label-caps" style={{ marginBottom: '8px', display: 'block' }}>Attendance Date</label>
                    <div style={{ position: 'relative' }}>
                        <Calendar size={18} style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                        <input 
                            className="input-field" 
                            style={{ paddingLeft: '40px', marginBottom: 0 }} 
                            type="date" 
                            value={selectedDate} 
                            onChange={(e) => setSelectedDate(e.target.value)} 
                        />
                    </div>
                </div>

                {attendances.length > 0 && (
                    <div style={{ display: 'flex', gap: '24px', background: 'var(--surface-variant)', padding: '12px 24px', borderRadius: '12px', border: '1px solid var(--outline)' }}>
                        <div>
                            <p className="text-label-caps" style={{ fontSize: '10px' }}>Recorded</p>
                            <p style={{ fontWeight: 700, fontSize: '18px' }}>{attendances.length}</p>
                        </div>
                        <div>
                            <p className="text-label-caps" style={{ fontSize: '10px' }}>Total Wage</p>
                            <p style={{ fontWeight: 700, fontSize: '18px', color: 'var(--success)' }}>฿{totalRecordedWage.toLocaleString()}</p>
                        </div>
                    </div>
                )}
            </div>

            {!selectedProject ? (
                <div className="glass-panel" style={{ padding: '80px', textAlign: 'center' }}>
                    <FileSpreadsheet size={48} color="var(--outline-variant)" style={{ marginBottom: '16px' }} />
                    <p className="text-h3" style={{ color: 'var(--text-muted)' }}>Select a project to record attendance</p>
                </div>
            ) : loading ? (
                <div className="grid">
                    {[1,2,3].map(i => <div key={i} className="skeleton-loader" style={{ height: '120px' }}></div>)}
                </div>
            ) : (
                <div style={{ display: 'grid', gap: '40px' }}>
                    {/* Already Recorded */}
                    {attendances.length > 0 && (
                        <div>
                            <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '16px' }}>
                                <div style={{ background: 'var(--success)', color: 'white', padding: '4px', borderRadius: '4px' }}>
                                    <UserCheck size={16} />
                                </div>
                                <h3 className="text-h3" style={{ fontSize: '18px' }}>Recorded Today</h3>
                            </div>
                            <div className="glass-panel table-wrapper">
                                <table className="modern-table">
                                    <thead>
                                        <tr><th>Worker Name</th><th>Check-in</th><th>Check-out</th><th>OT (hrs)</th><th style={{ textAlign: 'right' }}>Daily Wage</th></tr>
                                    </thead>
                                    <tbody>
                                        {attendances.map(a => (
                                            <tr key={a.id}>
                                                <td style={{ fontWeight: 600 }}>{a.workerName}</td>
                                                <td><div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}><Clock size={14} /> {a.checkIn}</div></td>
                                                <td><div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}><Clock size={14} /> {a.checkOut}</div></td>
                                                <td>{a.otHours}</td>
                                                <td style={{ textAlign: 'right', color: 'var(--success)', fontWeight: 700 }}>฿{a.dailyWage.toLocaleString()}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    )}

                    {/* Not Yet Recorded */}
                    {entries.length > 0 && (
                        <div>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
                                <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                                    <div style={{ background: 'var(--secondary)', color: 'white', padding: '4px', borderRadius: '4px' }}>
                                        <FileSpreadsheet size={16} />
                                    </div>
                                    <h3 className="text-h3" style={{ fontSize: '18px' }}>Pending Entry ({entries.length})</h3>
                                </div>
                                <div style={{ display: 'flex', gap: '16px', alignItems: 'center' }}>
                                    {entries.some(e => e.selected) && (
                                        <div style={{ textAlign: 'right' }}>
                                            <p className="text-label-caps" style={{ fontSize: '10px' }}>Selected Total</p>
                                            <p style={{ fontWeight: 700, color: 'var(--secondary)' }}>฿{totalSelectedWage.toLocaleString()}</p>
                                        </div>
                                    )}
                                    <button onClick={handleBulkSubmit} className="btn-primary" disabled={saving || !entries.some(e => e.selected)}>
                                        {saving ? <div className="spinner-small"></div> : <><Save size={18} /> Record Selected</>}
                                    </button>
                                </div>
                            </div>
                            <div className="glass-panel table-wrapper">
                                <table className="modern-table">
                                    <thead>
                                        <tr>
                                            <th style={{ width: '40px' }}>
                                                <button onClick={toggleAll} style={{ background: 'none', border: 'none', cursor: 'pointer', color: 'var(--secondary)' }}>
                                                    {entries.length > 0 && entries.every(e => e.selected) ? <CheckSquare size={20} /> : <Square size={20} />}
                                                </button>
                                            </th>
                                            <th>Worker Name</th>
                                            <th>Check-in</th>
                                            <th>Check-out</th>
                                            <th>OT (hrs)</th>
                                            <th style={{ textAlign: 'right' }}>Rate</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {entries.map((e, idx) => (
                                            <tr key={e.workerId} style={{ background: e.selected ? 'rgba(37, 99, 235, 0.03)' : 'transparent' }}>
                                                <td>
                                                    <button onClick={() => toggleEntry(idx)} style={{ background: 'none', border: 'none', cursor: 'pointer', color: e.selected ? 'var(--secondary)' : 'var(--outline-variant)' }}>
                                                        {e.selected ? <CheckSquare size={20} /> : <Square size={20} />}
                                                    </button>
                                                </td>
                                                <td style={{ fontWeight: 600 }}>{e.workerName}</td>
                                                <td><input className="input-field" style={{ width: '100px', padding: '4px 8px', marginBottom: 0 }} type="time" value={e.checkIn} onChange={(ev) => updateEntry(idx, 'checkIn', ev.target.value)} /></td>
                                                <td><input className="input-field" style={{ width: '100px', padding: '4px 8px', marginBottom: 0 }} type="time" value={e.checkOut} onChange={(ev) => updateEntry(idx, 'checkOut', ev.target.value)} /></td>
                                                <td><input className="input-field" style={{ width: '70px', padding: '4px 8px', marginBottom: 0 }} type="number" min="0" step="0.5" value={e.otHours} onChange={(ev) => updateEntry(idx, 'otHours', parseFloat(ev.target.value) || 0)} /></td>
                                                <td style={{ textAlign: 'right', color: 'var(--success)', fontWeight: 600 }}>฿{e.dailyWage.toLocaleString()}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    )}

                    {entries.length === 0 && attendances.length === 0 && (
                        <div className="glass-panel" style={{ padding: '80px', textAlign: 'center' }}>
                            <UserX size={48} color="var(--outline-variant)" style={{ marginBottom: '16px' }} />
                            <p style={{ color: 'var(--text-muted)' }}>No active workers found in the database. Add workers first.</p>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default AttendanceRecord;
