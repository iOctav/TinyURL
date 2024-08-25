import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi, describe, beforeEach, it, expect } from 'vitest'
import CreateShortURLForm from './CreateShortURLForm';
import { createShortURL } from '../../api/apiService';

vi.mock('../../api/apiService', () => ({
    createShortURL: vi.fn(),
}));

describe('CreateShortURLForm component', () => {
    const mockOnSuccess = vi.fn();

    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('renders the form correctly', () => {
        render(<CreateShortURLForm onSuccess={mockOnSuccess} />);

        expect(screen.getByLabelText('Long URL:')).toBeInTheDocument();
        expect(screen.getByLabelText('Custom Short URL (Optional):')).toBeInTheDocument();
        expect(screen.getByText('Create')).toBeInTheDocument();
    });

    it('handles input changes correctly', () => {
        render(<CreateShortURLForm onSuccess={mockOnSuccess} />);

        const longUrlInput = screen.getByLabelText('Long URL:');
        const customShortUrlInput = screen.getByLabelText('Custom Short URL (Optional):');

        fireEvent.change(longUrlInput, { target: { value: 'https://example.com' } });
        fireEvent.change(customShortUrlInput, { target: { value: 'custom-alias' } });

        expect(longUrlInput.value).toBe('https://example.com');
        expect(customShortUrlInput.value).toBe('custom-alias');
    });

    it('calls createShortURL and onSuccess on form submission', async () => {
        const mockResponse = { shortUrl: 'abc123', longUrl: 'https://example.com' };
        createShortURL.mockResolvedValueOnce(mockResponse);

        render(<CreateShortURLForm onSuccess={mockOnSuccess} />);

        const longUrlInput = screen.getByLabelText('Long URL:');
        fireEvent.change(longUrlInput, { target: { value: 'https://example.com' } });

        const submitButton = screen.getByText('Create');
        fireEvent.click(submitButton);

        expect(submitButton).toHaveTextContent('Creating...');

        await waitFor(() => {
            expect(createShortURL).toHaveBeenCalledWith('https://example.com', null);
            expect(mockOnSuccess).toHaveBeenCalledWith(mockResponse);
            expect(longUrlInput.value).toBe('');
        });
    });

    it('displays error message on failed submission', async () => {
        createShortURL.mockRejectedValueOnce(new Error('An error occurred'));

        render(<CreateShortURLForm onSuccess={mockOnSuccess} />);

        const longUrlInput = screen.getByLabelText('Long URL:');
        fireEvent.change(longUrlInput, { target: { value: 'https://example.com' } });

        const submitButton = screen.getByText('Create');
        fireEvent.click(submitButton);

        await waitFor(() => {
            expect(createShortURL).toHaveBeenCalledWith('https://example.com', null);
            expect(mockOnSuccess).not.toHaveBeenCalled();
            expect(screen.getByText('An error occurred')).toBeInTheDocument();
        });
    });

    it('disables the submit button while loading', async () => {
        createShortURL.mockResolvedValueOnce({ shortUrl: 'abc123', longUrl: 'https://example.com' });

        render(<CreateShortURLForm onSuccess={mockOnSuccess} />);

        const longUrlInput = screen.getByLabelText('Long URL:');
        fireEvent.change(longUrlInput, { target: { value: 'https://example.com' } });

        const submitButton = screen.getByText('Create');
        fireEvent.click(submitButton);

        expect(submitButton).toBeDisabled();

        await waitFor(() => {
            expect(submitButton).not.toBeDisabled();
        });
    });
});
