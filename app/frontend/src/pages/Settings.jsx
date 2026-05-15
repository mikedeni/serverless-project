import React, { useContext, useState } from 'react';
import { AuthContext } from '../contexts/AuthContext';
import { Save, Mail, User, Lock, Bell } from 'lucide-react';

const Settings = () => {
  const { user } = useContext(AuthContext);
  const [settings, setSettings] = useState({
    username: user?.username || '',
    email: user?.email || '',
    notifications: true,
    theme: 'light'
  });
  const [saveStatus, setSaveStatus] = useState('');

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setSettings(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleSave = () => {
    setSaveStatus('Saving...');
    setTimeout(() => {
      setSaveStatus('Settings saved successfully!');
      setTimeout(() => setSaveStatus(''), 3000);
    }, 500);
  };

  return (
    <div className="settings-container">
      <div className="settings-header">
        <h1>Settings</h1>
        <p>Manage your account preferences and application settings</p>
      </div>

      <div className="settings-content">
        {/* Profile Section */}
        <div className="settings-card">
          <div className="card-header">
            <User size={20} />
            <h2>Profile</h2>
          </div>
          <div className="form-group">
            <label>Username</label>
            <input
              type="text"
              name="username"
              value={settings.username}
              onChange={handleChange}
              className="form-input"
              disabled
            />
          </div>
          <div className="form-group">
            <label>Email</label>
            <input
              type="email"
              name="email"
              value={settings.email}
              onChange={handleChange}
              className="form-input"
            />
          </div>
          <p className="card-info">Role: <strong>{user?.role || 'Member'}</strong></p>
        </div>

        {/* Notifications Section */}
        <div className="settings-card">
          <div className="card-header">
            <Bell size={20} />
            <h2>Notifications</h2>
          </div>
          <div className="form-group checkbox">
            <input
              type="checkbox"
              id="notifications"
              name="notifications"
              checked={settings.notifications}
              onChange={handleChange}
            />
            <label htmlFor="notifications">Enable email notifications</label>
          </div>
          <p className="card-info">You'll receive updates about projects, invoices, and tasks</p>
        </div>

        {/* Appearance Section */}
        <div className="settings-card">
          <div className="card-header">
            <Lock size={20} />
            <h2>Appearance</h2>
          </div>
          <div className="form-group">
            <label>Theme</label>
            <select
              name="theme"
              value={settings.theme}
              onChange={handleChange}
              className="form-input"
            >
              <option value="light">Light</option>
              <option value="dark">Dark</option>
              <option value="auto">Auto</option>
            </select>
          </div>
        </div>

        {/* Save Section */}
        <div className="settings-actions">
          {saveStatus && (
            <div className={`status-message ${saveStatus.includes('Error') ? 'error' : 'success'}`}>
              {saveStatus}
            </div>
          )}
          <button onClick={handleSave} className="btn-primary">
            <Save size={18} />
            Save Changes
          </button>
        </div>
      </div>
    </div>
  );
};

export default Settings;
