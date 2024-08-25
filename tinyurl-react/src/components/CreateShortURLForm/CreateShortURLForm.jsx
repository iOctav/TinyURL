import { useState } from 'react';
import { createShortURL } from '../../api/apiService';
import './CreateShortURLForm.css';
import PropTypes from 'prop-types';

const CreateShortURLForm = ({ onSuccess }) => {
    const [longUrl, setLongUrl] = useState('');
    const [customShortUrl, setCustomShortUrl] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            const result = await createShortURL(longUrl, customShortUrl || null);
            onSuccess(result);
            setLongUrl('');
            setCustomShortUrl('');
        } catch (err) {
            setError(err.message || 'An error occurred');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="create-short-url-form">
            <h2>Create Short URL</h2>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label htmlFor="longUrl">Long URL:</label>
                    <input
                        type="url"
                        id="longUrl"
                        value={longUrl}
                        onChange={(e) => setLongUrl(e.target.value)}
                        required
                        placeholder="https://example.com/very/long/url"
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="customShortUrl">Custom Short URL (Optional):</label>
                    <input
                        type="text"
                        id="customShortUrl"
                        value={customShortUrl}
                        onChange={(e) => setCustomShortUrl(e.target.value)}
                        placeholder="custom-alias"
                    />
                </div>
                <button type="submit" disabled={loading}>
                    {loading ? 'Creating...' : 'Create'}
                </button>
                {error && <p className="error">{error}</p>}
            </form>
        </div>
    );
};

CreateShortURLForm.propTypes = {
    onSuccess: PropTypes.func.isRequired,
};

export default CreateShortURLForm;
