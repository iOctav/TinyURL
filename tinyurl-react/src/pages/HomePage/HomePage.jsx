import { useEffect, useState, useCallback } from 'react';
import CreateShortURLForm from '../../components/CreateShortURLForm/CreateShortURLForm';
import URLRequestForm from '../../components/URLRequestForm/URLRequestForm';
import URLList from '../../components/URLList/URLList';
import { getAllShortURLs } from '../../api/apiService';
import './HomePage.css';

const HomePage = () => {
    const [urls, setUrls] = useState([]);
    const [loading, setLoading] = useState(true);

    const fetchUrls = useCallback(async () => {
        setLoading(true);
        try {
            const response = await getAllShortURLs();
            setUrls(response.urls || []);
        } catch (error) {
            console.error('Error fetching URLs:', error);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchUrls();
    }, [fetchUrls]);

    return (
        <div className="home-page">
            <div className="forms-container">
                <CreateShortURLForm onSuccess={fetchUrls} />
                <URLRequestForm refreshUrls={fetchUrls} />
            </div>
            {loading ? (
                <p>Loading URLs...</p>
            ) : (
                <URLList urls={urls} refreshUrls={fetchUrls} />
            )}
        </div>
    );
};

export default HomePage;
