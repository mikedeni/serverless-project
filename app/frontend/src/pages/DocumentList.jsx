import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import api from '../utils/api';

export default function DocumentList() {
    const { id: projectId } = useParams();
    const [documents, setDocuments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [uploading, setUploading] = useState(false);
    
    // Form state
    const [file, setFile] = useState(null);
    const [category, setCategory] = useState('other');

    useEffect(() => {
        fetchDocuments();
    }, [projectId]);

    const fetchDocuments = async () => {
        try {
            setLoading(true);
            const response = await api.get(`/documents/project/${projectId}`);
            setDocuments(response || []);
            setError(null);
        } catch (err) {
            setError('Failed to load documents.');
        } finally {
            setLoading(false);
        }
    };

    const handleFileChange = (e) => {
        if (e.target.files && e.target.files.length > 0) {
            setFile(e.target.files[0]);
        }
    };

    const handleUpload = async (e) => {
        e.preventDefault();
        if (!file) return;

        const formData = new FormData();
        formData.append('File', file);
        formData.append('ProjectId', projectId);
        formData.append('Category', category);

        try {
            setUploading(true);
            await api.post('/documents', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
            setFile(null);
            document.getElementById('file-upload').value = '';
            await fetchDocuments();
        } catch (err) {
            alert('Upload failed: ' + (err.response?.data?.message || err.message));
        } finally {
            setUploading(false);
        }
    };

    const handleDelete = async (docId) => {
        if (!window.confirm('Are you sure you want to delete this document?')) return;
        try {
            await api.delete(`/documents/${docId}`);
            setDocuments(documents.filter(d => d.id !== docId));
        } catch (err) {
            alert('Failed to delete document.');
        }
    };

    const formatFileSize = (bytes) => {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    };

    const getFileIcon = (fileName) => {
        const ext = fileName.split('.').pop().toLowerCase();
        if (['jpg', 'jpeg', 'png', 'gif'].includes(ext)) return '🖼️';
        if (['pdf'].includes(ext)) return '📄';
        if (['doc', 'docx'].includes(ext)) return '📝';
        if (['xls', 'xlsx'].includes(ext)) return '📊';
        return '📁';
    };

    if (loading) return <div className="p-6 text-center">Loading documents...</div>;
    if (error) return <div className="p-6 text-red-500">{error}</div>;

    return (
        <div className="p-6 max-w-6xl mx-auto">
            <div className="flex justify-between items-center mb-6">
                <div>
                    <Link to={`/projects/${projectId}`} className="text-sm text-indigo-600 hover:underline mb-2 inline-block">
                        &larr; Back to Project
                    </Link>
                    <h1 className="text-2xl font-bold text-gray-800">Project Documents</h1>
                </div>
            </div>

            {/* Upload Form */}
            <div className="bg-white rounded-lg shadow p-6 mb-8">
                <h2 className="text-lg font-semibold text-gray-800 mb-4">Upload New Document</h2>
                <form onSubmit={handleUpload} className="flex flex-col md:flex-row gap-4 items-end">
                    <div className="flex-1 w-full">
                        <label className="block text-sm font-medium text-gray-700 mb-1">Select File</label>
                        <input 
                            id="file-upload"
                            type="file" 
                            onChange={handleFileChange}
                            required
                            className="w-full border border-gray-300 rounded-md p-2"
                        />
                    </div>
                    <div className="w-full md:w-48">
                        <label className="block text-sm font-medium text-gray-700 mb-1">Category</label>
                        <select 
                            value={category} 
                            onChange={(e) => setCategory(e.target.value)}
                            className="w-full border border-gray-300 rounded-md p-2"
                        >
                            <option value="contract">Contract</option>
                            <option value="blueprint">Blueprint</option>
                            <option value="permit">Permit</option>
                            <option value="receipt">Receipt</option>
                            <option value="report">Report</option>
                            <option value="other">Other</option>
                        </select>
                    </div>
                    <button 
                        type="submit" 
                        disabled={uploading || !file}
                        className="w-full md:w-auto bg-indigo-600 text-white px-6 py-2 rounded-md hover:bg-indigo-700 disabled:bg-gray-400"
                    >
                        {uploading ? 'Uploading...' : 'Upload'}
                    </button>
                </form>
            </div>

            {/* Document List */}
            <div className="bg-white rounded-lg shadow overflow-hidden">
                <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                        <tr>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">File</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Category</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Size</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Uploaded By</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
                            <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                        {documents.length === 0 ? (
                            <tr>
                                <td colSpan="6" className="px-6 py-4 text-center text-gray-500">
                                    No documents uploaded yet.
                                </td>
                            </tr>
                        ) : documents.map(doc => (
                            <tr key={doc.id} className="hover:bg-gray-50">
                                <td className="px-6 py-4 whitespace-nowrap">
                                    <div className="flex items-center">
                                        <span className="text-xl mr-2">{getFileIcon(doc.fileName)}</span>
                                        <a 
                                            href={`http://localhost:5032${doc.fileUrl}`} 
                                            target="_blank" 
                                            rel="noreferrer"
                                            className="text-indigo-600 hover:underline font-medium"
                                        >
                                            {doc.fileName}
                                        </a>
                                    </div>
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap">
                                    <span className="px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full bg-blue-100 text-blue-800 uppercase">
                                        {doc.category}
                                    </span>
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                    {formatFileSize(doc.fileSize)}
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                    {doc.uploadedByUserName || 'Unknown'}
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                    {new Date(doc.createdAt).toLocaleDateString()}
                                </td>
                                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                    <a 
                                        href={`http://localhost:5032${doc.fileUrl}`} 
                                        download
                                        target="_blank"
                                        rel="noreferrer"
                                        className="text-indigo-600 hover:text-indigo-900 mr-4"
                                    >
                                        Download
                                    </a>
                                    <button 
                                        onClick={() => handleDelete(doc.id)}
                                        className="text-red-600 hover:text-red-900"
                                    >
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
