import axios from 'axios';

const api = axios.create();

export const createShortURL = async (longUrl, customShortUrl) => {
    try {
        const response = await api.post('/v1/tiny-url/create', {
            longUrl,
            customShortUrl,
        });
        return response.data;
    } catch (error) {
        throw error.response.data;
    }
};

export const getLongURL = async (shortUrl) => {
    try {
        const response = await api.get(`/v1/tiny-url/${shortUrl}`);
        return response.data;
    } catch (error) {
        throw error.response.data;
    }
};

export const deleteShortURL = async (shortUrl) => {
    try {
        const response = await api.delete(`/v1/tiny-url/delete/${shortUrl}`);
        return response.data;
    } catch (error) {
        throw error.response.data;
    }
};

export const getURLStats = async (shortUrl) => {
    try {
        const response = await api.get(`/v1/tiny-url/${shortUrl}/stats`);
        return response.data;
    } catch (error) {
        throw error.response.data;
    }
};

export const getAllShortURLs = async (take = null, skip = null) => {
    try {
        const response = await api.get('/v1/tiny-url/all', {
            params: {
                'filter.take': take,
                'filter.skip': skip,
            },
        });
        return response.data;
    } catch (error) {
        throw error.response.data;
    }
};
