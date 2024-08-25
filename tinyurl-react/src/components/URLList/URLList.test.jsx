import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi } from 'vitest';
import URLList from './URLList';
import { deleteShortURL } from '../../api/apiService';
import { describe, beforeEach, it, expect } from 'vitest'

vi.mock('../../api/apiService', () => ({
    deleteShortURL: vi.fn(),
}));

describe('URLList component', () => {
    const mockUrls = [
        { shortUrl: 'abc123', longUrl: 'https://example.com/abc' },
        { shortUrl: 'def456', longUrl: 'https://example.com/def' },
    ];
    const mockRefreshUrls = vi.fn();

    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('renders the list of URLs correctly', () => {
        render(<URLList urls={mockUrls} refreshUrls={mockRefreshUrls} />);

        expect(screen.getByText('Your last 10 Short URLs')).toBeInTheDocument();
        expect(screen.getByText('https://example.com/abc')).toBeInTheDocument();
        expect(screen.getByText('https://example.com/def')).toBeInTheDocument();
        expect(screen.getByText(`${window.location.origin}/abc123`)).toBeInTheDocument();
        expect(screen.getByText(`${window.location.origin}/def456`)).toBeInTheDocument();
    });

    it('renders "No URLs created yet." when there are no URLs', () => {
        render(<URLList urls={[]} refreshUrls={mockRefreshUrls} />);

        expect(screen.getByText('No URLs created yet.')).toBeInTheDocument();
    });

    it('calls deleteShortURL and refreshUrls on successful deletion', async () => {
        deleteShortURL.mockResolvedValueOnce({});

        render(<URLList urls={mockUrls} refreshUrls={mockRefreshUrls} />);

        const deleteButton = screen.getAllByText('Delete')[0];

        vi.spyOn(window, 'confirm').mockReturnValue(true);
        vi.spyOn(window, 'alert').mockImplementation(() => {}); // Mock window.alert to prevent actual alerts

        fireEvent.click(deleteButton);

        await waitFor(() => {
            expect(deleteShortURL).toHaveBeenCalledWith('abc123');
            expect(mockRefreshUrls).toHaveBeenCalled();
            expect(window.alert).toHaveBeenCalledWith('URL deleted successfully');
        });
    });

    it('shows an error alert on failed deletion', async () => {
        deleteShortURL.mockRejectedValueOnce(new Error('Failed to delete URL'));

        render(<URLList urls={mockUrls} refreshUrls={mockRefreshUrls} />);

        const deleteButton = screen.getAllByText('Delete')[0];

        vi.spyOn(window, 'confirm').mockReturnValue(true);
        vi.spyOn(window, 'alert').mockImplementation(() => {}); // Mock window.alert to prevent actual alerts

        fireEvent.click(deleteButton);

        await waitFor(() => {
            expect(deleteShortURL).toHaveBeenCalledWith('abc123');
            expect(mockRefreshUrls).not.toHaveBeenCalled();
            expect(window.alert).toHaveBeenCalledWith('Failed to delete URL');
        });
    });

    it('does not delete URL if confirm is canceled', async () => {
        render(<URLList urls={mockUrls} refreshUrls={mockRefreshUrls} />);

        const deleteButton = screen.getAllByText('Delete')[0];

        vi.spyOn(window, 'confirm').mockReturnValue(false);

        fireEvent.click(deleteButton);

        expect(deleteShortURL).not.toHaveBeenCalled();
        expect(mockRefreshUrls).not.toHaveBeenCalled();
    });
});
