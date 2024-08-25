import { render, screen } from '@testing-library/react';
import { vi, describe, beforeEach, it, expect } from 'vitest'
import URLRequestForm from './URLRequestForm';

vi.mock('../../api/apiService', () => ({
    getLongURL: vi.fn(),
    deleteShortURL: vi.fn(),
}));

describe('URLRequestForm component', () => {
    const mockRefreshUrls = vi.fn();

    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('renders the form correctly', () => {
        render(<URLRequestForm refreshUrls={mockRefreshUrls} />);

        expect(screen.getByLabelText('Short URL:')).toBeInTheDocument();
        expect(screen.getByText('Get Long URL')).toBeInTheDocument();
    });
});
