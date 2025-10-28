import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

export const options = {
    stages: [
        { duration: '10s', target: 1000 },
        { duration: '20s', target: 1000 },
        { duration: '35s', target: 3000 },
        { duration: '5s', target: 0 },
    ],
};

const BASE_URL = 'http://0.0.0.0:5077/api/productservice';

export default function (data) {
    let res = http.get(url, {
        headers: { 'Content-Type': 'application/json' }
    });

    check(res, {
        'status is 200': (r) => r.status === 200,
        'response not empty': (r) => r.body.length > 0,
    });

    sleep(1);
}