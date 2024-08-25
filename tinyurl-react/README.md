# TinyURL-style service

This is a React-based web application for managing short URLs, similar to TinyURL. The application allows you to create, view, delete, and track statistics for short URLs.

## Getting Started

### Prerequisites

Ensure you have the following installed on your system:

- Node.js (v12 or higher)

## Server Configuration and Proxy Setup

### Default Server Location

By default, the TinyURL-style service is configured to communicate with a backend server located at `http://localhost:9090`. This server is expected to handle all API requests, such as creating, retrieving, deleting, and tracking short URLs.

### Proxy Configuration

To streamline development and avoid Cross-Origin Resource Sharing (CORS) issues, the application uses a proxy configuration. This setup ensures that API requests from the front-end are forwarded to the backend server seamlessly.

The proxy settings are defined in the `vite.config.js` file located in the root directory of the project.

#### Example of Default Proxy Configuration:

```javascript
// vite.config.js
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/v1/tiny-url': {
        target: 'http://localhost:9090',
        changeOrigin: true,
      },
    },
  },
})
