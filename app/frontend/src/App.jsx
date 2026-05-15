import React, { useContext } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, AuthContext } from './contexts/AuthContext';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import ProjectList from './pages/ProjectList';
import ProjectDetail from './pages/ProjectDetail';
import AddExpense from './pages/AddExpense';
import CreateProject from './pages/CreateProject';
import QuotationList from './pages/QuotationList';
import CreateQuotation from './pages/CreateQuotation';
import QuotationDetail from './pages/QuotationDetail';
import WorkerList from './pages/WorkerList';
import AttendanceRecord from './pages/AttendanceRecord';
import MaterialList from './pages/MaterialList';
import MaterialDetail from './pages/MaterialDetail';
import InvoiceList from './pages/InvoiceList';
import InvoiceDetail from './pages/InvoiceDetail';
import ProjectReport from './pages/ProjectReport';
import DailyReportList from './pages/DailyReportList';
import SubcontractorList from './pages/SubcontractorList';
import NotificationList from './pages/NotificationList';
import DocumentList from './pages/DocumentList';
import GlobalInvoices from './pages/GlobalInvoices';
import Settings from './pages/Settings';
import './App.css';
import MainLayout from './components/MainLayout';
import { Outlet } from 'react-router-dom';

const ProtectedRoute = ({ children }) => {
  const { isAuthenticated } = useContext(AuthContext);
  if (!isAuthenticated) return <Navigate to="/login" />;
  
  return (
    <MainLayout>
      {children || <Outlet />}
    </MainLayout>
  );
};

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<Navigate to="/dashboard" />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          
          <Route element={<ProtectedRoute />}>
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/projects" element={<ProjectList />} />
            <Route path="/projects/:id" element={<ProjectDetail />} />
            <Route path="/projects/new" element={<CreateProject />} />
            <Route path="/projects/:id/expenses/add" element={<AddExpense />} />
            <Route path="/projects/:projectId/quotations" element={<QuotationList />} />
            <Route path="/projects/:projectId/quotations/new" element={<CreateQuotation />} />
            <Route path="/quotations/:id" element={<QuotationDetail />} />
            <Route path="/workers" element={<WorkerList />} />
            <Route path="/attendance" element={<AttendanceRecord />} />
            <Route path="/materials" element={<MaterialList />} />
            <Route path="/materials/:id" element={<MaterialDetail />} />
            <Route path="/projects/:projectId/invoices" element={<InvoiceList />} />
            <Route path="/invoices/:id" element={<InvoiceDetail />} />
            <Route path="/projects/:projectId/report" element={<ProjectReport />} />
            <Route path="/projects/:projectId/daily-reports" element={<DailyReportList />} />
            <Route path="/invoices" element={<GlobalInvoices />} />
            <Route path="/subcontractors" element={<SubcontractorList />} />
            <Route path="/notifications" element={<NotificationList />} />
            <Route path="/projects/:id/documents" element={<DocumentList />} />
            <Route path="/settings" element={<Settings />} />
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
