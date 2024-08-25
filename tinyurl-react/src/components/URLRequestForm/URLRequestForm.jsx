import { useState } from 'react';
import { getLongURL, deleteShortURL } from '../../api/apiService';
import './URLRequestForm.css';
import PropTypes from 'prop-types';

const URLRequestForm = ({ refreshUrls }) => {
    const [shortUrl, setShortUrl] = useState('');
    const [longUrl, setLongUrl] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [successMessage, setSuccessMessage] = useState(null);

    const handleFetchLongURL = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        setLongUrl(null);
        setSuccessMessage(null);

        try {
            const cleanedShortUrl = shortUrl.replace(window.location.origin, '');
            const result = await getLongURL(cleanedShortUrl);
            setLongUrl(result.longUrl);
        } catch (err) {
            setError(err.message || 'Failed to retrieve long URL');
        } finally {
            setLoading(false);
        }
    };

    const handleDeleteShortURL = async () => {
        setLoading(true);
        setError(null);
        setSuccessMessage(null);

        try {
            const cleanedShortUrl = shortUrl.replace(window.location.origin, '');
            await deleteShortURL(cleanedShortUrl);
            setSuccessMessage('Short URL successfully deleted');
            setLongUrl(null);
            refreshUrls();
        } catch (err) {
            setError(err.message || 'Failed to delete short URL');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="url-request-form">
            <h2>Request Long URL</h2>
            <form onSubmit={handleFetchLongURL}>
                <div className="form-group">
                    <label htmlFor="shortUrl">Short URL:</label>
                    <input
                        type="text"
                        id="shortUrl"
                        value={shortUrl}
                        onChange={(e) => setShortUrl(e.target.value)}
                        required
                        placeholder="Enter short URL identifier"
                    />
                </div>
                <button type="submit" disabled={loading}>
                    {loading ? 'Fetching...' : 'Get Long URL'}
                </button>
            </form>
            {error && <p className="error">{error}</p>}
            {longUrl && (
                <div className="url-result">
                    <p><strong>Long URL:</strong> {longUrl}</p>
                    <button onClick={handleDeleteShortURL} disabled={loading}>
                        {loading ? 'Deleting...' : 'Delete Short URL'}
                    </button>
                    {successMessage && <p className="success">{successMessage}</p>}
                </div>
            )}
        </div>
    );
};

URLRequestForm.propTypes = {
    refreshUrls: PropTypes.func.isRequired,
};

export default URLRequestForm;
