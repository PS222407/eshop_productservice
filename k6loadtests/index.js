import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// Custom metrics
const errorRate = new Rate('errors');

// Test configuration
export const options = {
    stages: [
        { duration: '10s', target: 1000 },
        { duration: '20s', target: 1000 },
        { duration: '35s', target: 3000 },
        { duration: '5s', target: 0 },
    ],
    thresholds: {
        http_req_duration: ['p(95)<500', 'p(99)<1000'], // 95% < 500ms, 99% < 1000ms
        http_req_failed: ['rate<0.01'],                  // Error rate < 1%
        errors: ['rate<0.1'],                            // Custom error rate < 10%
    },
};

// Base URL - adjust to your API
const BASE_URL = 'http://192.168.49.2/api/productservice/v1';

// // Test data
// const testData = {
//     users: [
//         { username: 'user1@example.com', password: 'Test123!' },
//         { username: 'user2@example.com', password: 'Test123!' },
//     ],
// };
//
// export function setup() {
//     // Setup code - runs once before tests
//     console.log('Starting load test...');
//
//     // You can authenticate here and return tokens for VUs to use
//     const loginRes = http.post(`${BASE_URL}/auth/login`, JSON.stringify({
//         username: testData.users[0].username,
//         password: testData.users[0].password,
//     }), {
//         headers: { 'Content-Type': 'application/json' },
//     });
//
//     if (loginRes.status === 200) {
//         const token = loginRes.json('token');
//         return { token };
//     }
//
//     return {};
// }

export default function (data) {
    const params = {
        headers: {
            'Content-Type': 'application/json',
            // 'Authorization': data.token ? `Bearer ${data.token}` : '',
        },
    };

    // Test 1: GET endpoint
    let response = http.get(`${BASE_URL}/Category`, params);
    check(response, {
        'GET /Category status is 200': (r) => r.status === 200,
        'GET /Category response time < 500ms': (r) => r.timings.duration < 500,
    }) || errorRate.add(1);

    sleep(1);

    // Test 2: GET by ID
    response = http.get(`${BASE_URL}/Category/0199708c-4fe1-7fe0-a1f0-7bf8ae4ae78e`, params);
    check(response, {
        'GET /Category/:id status is 200': (r) => r.status === 200,
        'GET /Category/:id has data': (r) => r.json('id') !== undefined,
    }) || errorRate.add(1);

    sleep(1);
}

export function teardown(data) {
    // Cleanup code - runs once after all tests
    console.log('Load test completed');
}