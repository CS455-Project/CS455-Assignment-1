const request = require('supertest');
const mongoose = require('mongoose');
const { MongoMemoryServer } = require('mongodb-memory-server');
// const app = require('../server/server.js'); // Adjust this path to where your server file is located


jest.mock('../server', () => {
    const originalModule = jest.requireActual('../server');
    const app = originalModule.app;
    return { app };
  });

const { app } = require('../server');

let mongoServer;

beforeAll(async () => {
  mongoServer = await MongoMemoryServer.create();
  const mongoUri = mongoServer.getUri();
  await mongoose.connect(mongoUri, {
    useNewUrlParser: true,
    useUnifiedTopology: true,
    serverSelectionTimeoutMS: 10000 // Adjust timeout as needed
  });
  
});

afterAll(async () => {
  await mongoose.disconnect();
  await mongoServer.stop();
});

const userSchema = new mongoose.Schema({
    name: String,
    score: Number
  });
  
const User = mongoose.model('User', userSchema);

describe('Server Endpoints', () => {
    beforeEach(async () => {
        await User.deleteMany(); // clear test data before each test
      });

//   describe('POST /', () => {
//     it('should create a new user with valid data', async () => {
//       const response = await request(app)
//         .post('/')
//         .send({ name: 'TestUser', score: 100 })
//         .expect(200);

//       expect(response.body).toHaveProperty('name', 'TestUser');
//       expect(response.body).toHaveProperty('score', 100);
//     });

//     it('should return 400 for invalid score data', async () => {
//       const response = await request(app)
//         .post('/')
//         .send({ name: 'TestUser' })
//         .expect(400);

//       expect(response.body).toHaveProperty('error', 'Invalid score data');
//     });
//   });

//   describe('GET /leaderboard', () => {
//     beforeEach(async () => {
//       // Add some test data
//       await mongoose.model('User').create([
//         { name: 'User1', score: 100 },
//         { name: 'User2', score: 200 },
//         { name: 'User3', score: 150 }
//       ]);
//     });

//     afterEach(async () => {
//       await User.deleteMany({});
//     });

//     it('should return top 10 scores sorted in descending order', async () => {
//       const response = await request(app)
//         .get('/leaderboard')
//         .expect(200);

//       expect(response.body).toHaveLength(3);
//       expect(response.body[0]).toHaveProperty('name', 'User2');
//       expect(response.body[0]).toHaveProperty('score', 200);
//       expect(response.body).not.toHaveProperty('_id');
//     });
//   });

  describe('GET /', () => {
    it('should return welcome message', async () => {
      const response = await request(app)
        .get('/')
        .expect(200);

      expect(response.text).toContain("Welcome to the server side of Whack'Em-All !!!");
    });
  });
});

    afterAll(done => {
        // Closing the DB connection allows Jest to exit successfully.
        mongoose.connection.close();
        done();
    });