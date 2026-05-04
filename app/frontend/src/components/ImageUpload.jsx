import React, { useState, useRef } from 'react';
import { Camera, Upload, X, Check, Loader2 } from 'lucide-react';
import api from '../utils/api';

const ImageUpload = ({ entityType, entityId, currentImage, onUploadSuccess }) => {
    const [uploading, setUploading] = useState(false);
    const [preview, setPreview] = useState(null);
    const [error, setError] = useState(null);
    const fileInputRef = useRef(null);

    const handleFileChange = async (e) => {
        const file = e.target.files[0];
        if (!file) return;

        // Preview
        const reader = new FileReader();
        reader.onloadend = () => {
            setPreview(reader.result);
        };
        reader.readAsDataURL(file);

        // Upload
        setUploading(true);
        setError(null);

        const formData = new FormData();
        formData.append('file', file);

        try {
            const response = await api.post(`/${entityType}/${entityId}/image`, formData);
            
            if (onUploadSuccess) {
                onUploadSuccess(response.imageUrl);
            }
            setPreview(null); // Clear preview once uploaded and state updated from parent
        } catch (err) {
            console.error("Upload failed", err);
            setError("Failed to upload image");
        } finally {
            setUploading(false);
        }
    };

    const triggerUpload = () => {
        fileInputRef.current.click();
    };

    const imageUrl = preview || (currentImage ? currentImage : null);

    return (
        <div className="image-upload-container" style={{ position: 'relative', width: 'fit-content' }}>
            <input 
                type="file" 
                ref={fileInputRef} 
                onChange={handleFileChange} 
                accept="image/*" 
                style={{ display: 'none' }} 
            />
            
            <div 
                onClick={triggerUpload}
                style={{ 
                    width: '120px', 
                    height: '120px', 
                    borderRadius: '16px', 
                    border: '2px dashed var(--outline)', 
                    display: 'flex', 
                    flexDirection: 'column',
                    alignItems: 'center', 
                    justifyContent: 'center',
                    cursor: 'pointer',
                    overflow: 'hidden',
                    position: 'relative',
                    background: 'var(--surface-variant)',
                    transition: 'all 0.2s ease'
                }}
                className="hover-scale"
            >
                {imageUrl ? (
                    <img 
                        src={imageUrl} 
                        alt="Preview" 
                        style={{ width: '100%', height: '100%', objectFit: 'cover' }} 
                    />
                ) : (
                    <div style={{ textAlign: 'center', color: 'var(--text-muted)' }}>
                        <Camera size={24} style={{ marginBottom: '8px' }} />
                        <span style={{ fontSize: '12px', fontWeight: 600 }}>ADD PHOTO</span>
                    </div>
                )}

                {uploading && (
                    <div style={{ 
                        position: 'absolute', 
                        inset: 0, 
                        background: 'rgba(0,0,0,0.5)', 
                        display: 'flex', 
                        alignItems: 'center', 
                        justifyContent: 'center' 
                    }}>
                        <Loader2 size={24} className="spinner" color="white" />
                    </div>
                )}
            </div>

            {imageUrl && !uploading && (
                <button 
                    onClick={triggerUpload}
                    style={{ 
                        position: 'absolute', 
                        bottom: '-10px', 
                        right: '-10px', 
                        background: 'var(--primary)', 
                        color: 'white', 
                        border: 'none', 
                        borderRadius: '50%', 
                        width: '32px', 
                        height: '32px', 
                        display: 'flex', 
                        alignItems: 'center', 
                        justifyContent: 'center',
                        boxShadow: '0 4px 8px rgba(0,0,0,0.2)',
                        cursor: 'pointer'
                    }}
                >
                    <Upload size={16} />
                </button>
            )}

            {error && (
                <div style={{ color: 'var(--error)', fontSize: '10px', marginTop: '4px', position: 'absolute', width: '150px' }}>
                    {error}
                </div>
            )}
        </div>
    );
};

export default ImageUpload;
