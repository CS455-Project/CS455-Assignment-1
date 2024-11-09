const express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');

const app = express();

// Define your backend servers
const servers = [
    'http://localhost:8001',  // First server instance
    'http://localhost:8000'   // Second server instance
];

let currentServer = 0;

// Middleware to handle CORS
app.use((req, res, next) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Methods', 'GET, POST, OPTIONS');
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept');
    next();
});

// Simple health check endpoint
app.get('/health', (req, res) => {
    res.send({
        status: 'healthy',
        servers: servers,
        currentServer: currentServer
    });
});

// Round-robin load balancing logic
const loadBalancer = createProxyMiddleware({
    router: (req) => {
        // Select server in round-robin fashion
        const server = servers[currentServer];
        
        // Update current server for next request
        currentServer = (currentServer + 1) % servers.length;
        
        console.log(`Routing request to: ${server} (Server ${currentServer + 1})`);
        return server;
    },
    changeOrigin: true,
    onProxyReq: (proxyReq, req, res) => {
        // Add custom header to track which backend server handled the request
        proxyReq.setHeader('X-Server-ID', `Server-${currentServer + 1}`);
    },
    onError: (err, req, res) => {
        console.error('Proxy Error:', err);
        res.status(500).send('Proxy Error');
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

// Periodically log load balancing stats
setInterval(() => {
    console.log('\nLoad Balancer Status:');
    console.log('Current server index:', currentServer);
    console.log('Available servers:', servers);
}, 10000);