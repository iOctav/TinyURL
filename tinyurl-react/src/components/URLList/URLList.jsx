import { deleteShortURL } from '../../api/apiService';
import './URLList.css';
import PropTypes from 'prop-types';

const URLList = ({ urls, refreshUrls }) => {
    const handleDelete = async (shortUrl) => {
        if (window.confirm('Are you sure you want to delete this URL?')) {
            try {
                await deleteShortURL(shortUrl);
                refreshUrls();
                alert('URL deleted successfully');
            } catch (error) {
                alert(error.message || 'Failed to delete URL');
            }
        }
    };

    return (
        <div className="url-list">
            <h2>Your last 10 Short URLs</h2>
            {urls.length === 0 ? (
                <p>No URLs created yet.</p>
            ) : (
                <table>
                    <thead>
                    <tr>
                        <th>Short URL</th>
                        <th>Long URL</th>
                        <th>Actions</th>
                    </tr>
                    </thead>
                    <tbody>
                    {urls.map((url) => (
                        <tr key={url.shortUrl}>
                            <td>
                                <a
                                    href={url.longUrl}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                >
                                    {`${window.location.origin}/${url.shortUrl}`}
                                </a>
                            </td>
                            <td className="long-url">{url.longUrl}</td>
                            <td>
                                <button
                                    className="delete-button"
                                    onClick={() => handleDelete(url.shortUrl)}
                                >
                                    Delete
                                </button>
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

URLList.propTypes = {
    urls: PropTypes.arrayOf(
        PropTypes.shape({
            shortUrl: PropTypes.string.isRequired,
            longUrl: PropTypes.string.isRequired,
        })
    ),
    refreshUrls: PropTypes.func,
};

export default URLList;
