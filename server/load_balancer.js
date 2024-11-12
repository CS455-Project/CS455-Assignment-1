const express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');

const app = express();

// Define your backend servers
const servers = [
    'https://cs455-assignment-1.onrender.com',  // First server instance
    'https://server-2-0sir.onrender.com'   // Second server instance
];

let currentServer = 0;

// Health check for backend servers
const checkServerHealth = async (url) => {
    try {
        const response = await fetch(`${url}/`);
        return response.ok;
    } catch (error) {
        return false;
    }
};

// Middleware to handle CORS
app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Methods', 'GET, POST, OPTIONS');
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept');
    next();
});

// Round-robin load balancing logic with health check
const loadBalancer = createProxyMiddleware({
    target: servers[0],
    changeOrigin: true,
    router: async (req) => {
        let attempts = 0;
        const maxAttempts = servers.length;

        while (attempts < maxAttempts) {
            const server = servers[currentServer];
            const isHealthy = await checkServerHealth(server);

            if (isHealthy) {
                console.log(`Routing request to: ${server}`);
                currentServer = (currentServer + 1) % servers.length;
                return server;
            }

            // Try next server
            currentServer = (currentServer + 1) % servers.length;
            attempts++;
        }

        throw new Error('No healthy servers available');
    },
    onError: (err, req, res) => {
        console.error('Proxy Error:', err);
        res.status(500).send('No healthy servers available');
    }
});

// Apply the load balancer middleware to all routes
app.use('/', loadBalancer);

// Start the load balancer
const LOAD_BALANCER_PORT = 3000;
app.listen(LOAD_BALANCER_PORT, () => {
    console.log(`Load balancer running on port ${LOAD_BALANCER_PORT}`);
    console.log('Distributing traffic between:', servers);
});