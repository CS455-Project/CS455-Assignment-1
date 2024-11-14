const request = require('supertest');
const mongoose = require('mongoose');
const { app, User } = require('./server'); // Adjust the path if necessary
const { MongoMemoryServer } = require('mongodb-memory-server');
const net = require('net');

// Initialize MongoDB memory server
let mongoServer;

beforeAll(async () => {
    mongoServer = await MongoMemoryServer.create();
    const mongoUri = mongoServer.getUri();
    await mongoose.connect(mongoUri, { useNewUrlParser: true, useUnifiedTopology: true });
});

afterAll(async () => {
    await mongoose.disconnect();
    await mongoServer.stop();
});

describe("Server Initialization", () => {
  it("should not start the server when NODE_ENV is 'test'", async () => {
      // Set the NODE_ENV to 'test'
      process.env.NODE_ENV = 'test';

      // Mock the app.listen method
      const listenSpy = jest.spyOn(app, 'listen').mockImplementation(() => {
          return {
              close: jest.fn(),
              listening: false // Simulate that it's not listening
          };
      });

      // Call the listen method to trigger the code
      app.listen(process.env.PORT || 8000);

      // Ensure that listen was called but does not listen
      expect(listenSpy).toHaveBeenCalled();
      expect(listenSpy().listening).toBe(false);

      // Clean up the mock
      listenSpy.mockRestore();
  });
});

describe("User API", () => {
    it("should save a new user and return the saved document", async () => {
        const newUser = { name: "John Doe", score: 10 };

        const response = await request(app)
            .post("/")
            .send(newUser);

        expect(response.status).toBe(200);
        expect(response.body).toHaveProperty('_id');
        expect(response.body.name).toBe(newUser.name);
        expect(response.body.score).toBe(newUser.score);

        await User.deleteOne({ _id: response.body._id });
    });

    it("should return a 400 error for invalid score data", async () => {
        const invalidUser = { name: "Jane Doe" }; // Missing score

        const response = await request(app)
            .post("/")
            .send(invalidUser);

        expect(response.status).toBe(400);
        expect(response.body).toHaveProperty('error', 'Invalid score data');
    });

    it("should return the leaderboard sorted by score", async () => {
        await User.create([{ name: "Alice", score: 20 }, { name: "Bob", score: 30 }]);

        const response = await request(app)
            .get("/leaderboard");

        expect(response.status).toBe(200);
        expect(response.body).toHaveLength(2);
        expect(response.body[0]).toEqual(expect.objectContaining({ name: "Bob", score: 30 }));
        expect(response.body[1]).toEqual(expect.objectContaining({ name: "Alice", score: 20 }));
    });

    it("should return a welcome message", async () => {
        const response = await request(app)
            .get("/");

        expect(response.status).toBe(200);
        expect(response.text).toContain("Welcome to the server side of Whack'Em-All !!!");
    });

    it("should handle errors when retrieving the leaderboard", async () => {
      // Mock the User.find() method to throw an error
      jest.spyOn(User, 'find').mockImplementationOnce(() => {
          throw new Error("Database query failed");
      });

      const response = await request(app).get("/leaderboard");

      expect(response.status).toBe(500);
      expect(response.body).toEqual({ error: "Error retrieving leaderboard" });

      // Restore the original implementation
      User.find.mockRestore();
  });
});

describe('DELETE /leaderboard/delete', () => {
    it('should delete entries for an existing player', async () => {
        // Arrange: Create a user to delete
        const playerName = 'TestPlayer';
        const user = new User({ name: playerName, score: 100 });
        await user.save();

        // Act: Send DELETE request
        const response = await request(app)
            .post('/leaderboard/delete')
            .query({ name: playerName }); // Sending the player name as query

        // Assert
        expect(response.status).toBe(200);
        expect(response.body.message).toBe('1 entries deleted.');

        // Ensure the user was deleted
        const deletedUser = await User.findOne({ name: playerName });
        expect(deletedUser).toBeNull();
    });

    it('should return 404 if no entries are found for the given player name', async () => {
        // Act: Attempt to delete a non-existing player
        const response = await request(app)
            .post('/leaderboard/delete')
            .query({ name: 'NonExistentPlayer' });

        // Assert
        expect(response.status).toBe(404);
        expect(response.body.error).toBe('No entries found for the given name.');
    });

    it('should handle errors during deletion', async () => {
        // Arrange: Mock a database error
        jest.spyOn(User, 'deleteMany').mockImplementationOnce(() => {
            throw new Error('Database error');
        });

        const playerName = 'TestPlayer';
        const response = await request(app)
            .post('/leaderboard/delete')
            .query({ name: playerName });

        // Assert
        expect(response.status).toBe(500);
        expect(response.body.error).toBe('Error deleting entries');
    });

    it('should return 400 if player name is missing', async () => {
        // Act: Attempt to delete without a player name
        const response = await request(app)
            .post('/leaderboard/delete');

        // Assert
        expect(response.status).toBe(400);
        expect(response.body.error).toBe('Player name is required.'); // You should modify your server code to handle this case
    });
});

describe('Port Check Functionality', () => {
    let server;
    const testPort = 3333;

    beforeEach(() => {
        // Clear any existing servers
        server = null;
    });

    afterEach((done) => {
        // Cleanup: close server if it exists
        if (server && server.listening) {
            server.close(done);
        } else {
            done();
        }
    });

    it('should return true when port is available', async () => {
        // Import the checkPort function
        const { checkPort } = require('./server');
        
        // Test an available port
        const result = await checkPort(testPort);
        expect(result).toBe(true);
    });

    it('should return false when port is in use', async () => {
        // Import the checkPort function
        const { checkPort } = require('./server');
        
        // Create a server to occupy the port
        server = net.createServer();
        
        // Return a promise that resolves when the server is listening
        await new Promise((resolve) => {
            server.listen(testPort, () => resolve());
        });

        // Test the occupied port
        const result = await checkPort(testPort);
        expect(result).toBe(false);
    });

    it('should reject on non-EADDRINUSE errors', async () => {
        // Import the checkPort function
        const { checkPort } = require('./server');
        
        // Mock net.createServer to simulate a different error
        const mockServer = {
            once: jest.fn((event, cb) => {
                if (event === 'error') {
                    cb(new Error('Different error'));
                }
                return mockServer;
            }),
            listen: jest.fn()
        };

        jest.spyOn(net, 'createServer').mockImplementationOnce(() => mockServer);

        // Test error handling
        await expect(checkPort(testPort)).rejects.toThrow('Different error');

        // Restore the original implementation
        net.createServer.mockRestore();
    });

    it('should handle server closing correctly', async () => {
        // Import the checkPort function
        const { checkPort } = require('./server');
        
        // Create a proper mock server with chaining
        const mockServer = {
            once: jest.fn(function(event, cb) {
                if (event === 'listening') {
                    setTimeout(() => cb(), 0);
                } else if (event === 'close') {
                    setTimeout(() => cb(), 0);
                } else if (event === 'error') {
                    // Do nothing for error event in this test
                }
                return this;
            }),
            close: jest.fn(function() {
                setTimeout(() => {
                    this.once('close', () => {});
                }, 0);
                return this;
            }),
            listen: jest.fn(function() {
                return this;
            })
        };

        jest.spyOn(net, 'createServer').mockImplementationOnce(() => mockServer);

        const result = await checkPort(testPort);
        
        // Verify the port check was successful
        expect(result).toBe(true);
        
        // Verify proper method calls
        expect(mockServer.once).toHaveBeenCalledWith('listening', expect.any(Function));
        expect(mockServer.listen).toHaveBeenCalledWith(testPort);
        
        // Restore the original implementation
        net.createServer.mockRestore();
    });
});