import { useState } from 'react';
import { getURLStats } from '../../api/apiService';
import './URLStats.css';

const URLStats = () => {
    const [shortUrl, setShortUrl] = useState('');
    const [stats, setStats] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleGetStats = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        setStats(null);

        const cleanedShortUrl = shortUrl.replace(window.location.origin, '');

        try {
            const result = await getURLStats(cleanedShortUrl);
            setStats(result);
        } catch (err) {
            setError(err.message || 'Failed to retrieve stats');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="url-stats">
            <h2>URL Statistics</h2>
            <form onSubmit={handleGetStats}>
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
                    {loading ? 'Fetching...' : 'Get Stats'}
                </button>
            </form>
            {error && <p className="error">{error}</p>}
            {stats && (
                <div className="stats-result">
                    <p>
                        <strong>Short URL:</strong>{' '}
                        {`${window.location.origin}/${shortUrl.replace(window.location.origin, '')}`}
                    </p>
                    <p>
                        <strong>Click Count:</strong> {stats.clickCount}
                    </p>
                </div>
            )}
        </div>
    );
};

export default URLStats;
