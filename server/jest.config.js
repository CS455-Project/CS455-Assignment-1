module.exports = {
    testEnvironment: 'node',
    coverageDirectory: './coverage',
    collectCoverageFrom: ['./server.js'],
    coverageReporters: ['lcov', 'text', 'text-summary'],
    testMatch: ['**/__tests__/**/*.js', '**/?(*.)+(spec|test).js'],
    verbose: true,
    testTimeout: 3000,
    // coveragePathIgnorePatterns: [
    //   "./.*", // Ignore everything by default
    // ],
  };