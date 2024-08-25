import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi, describe, beforeEach, it, expect } from 'vitest'
import URLStats from './URLStats';
import { getURLStats } from '../../api/apiService';

vi.mock('../../api/apiService', () => ({
    getURLStats: vi.fn(),
}));

describe('URLStats component', () => {

    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('renders the form correctly', () => {
        render(<URLStats />);

        expect(screen.getByLabelText('Short URL:')).toBeInTheDocument();
        expect(screen.getByText('Get Stats')).toBeInTheDocument();
    });

    it('displays error message when fetching stats fails', async () => {
        getURLStats.mockRejectedValueOnce(new Error('Failed to retrieve stats'));

        render(<URLStats />);

        const shortUrlInput = screen.getByLabelText('Short URL:');
        fireEvent.change(shortUrlInput, { target: { value: 'abc123' } });

        const submitButton = screen.getByText('Get Stats');
        fireEvent.click(submitButton);

        await waitFor(() => {
            expect(getURLStats).toHaveBeenCalledWith('abc123');
            expect(screen.getByText('Failed to retrieve stats')).toBeInTheDocument();
        });
    });

    it('disables the submit button while loading', async () => {
        getURLStats.mockResolvedValueOnce({ clickCount: 42 });

        render(<URLStats />);

        const shortUrlInput = screen.getByLabelText('Short URL:');
        fireEvent.change(shortUrlInput, { target: { value: 'abc123' } });

        const submitButton = screen.getByText('Get Stats');
        fireEvent.click(submitButton);

        expect(submitButton).toBeDisabled();

        await waitFor(() => {
            expect(submitButton).not.toBeDisabled();
        });
    });
});
