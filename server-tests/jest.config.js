module.exports = {
    testEnvironment: 'node',
    coverageDirectory: './coverage',
    collectCoverageFrom: ['**/*.js', '!**/node_modules/**', '!**/vendor/**'],
    coverageReporters: ['lcov', 'text', 'text-summary'],
    testMatch: ['**/__tests__/**/*.js', '**/?(*.)+(spec|test).js'],
    verbose: true,
    testTimeout: 3000
  };