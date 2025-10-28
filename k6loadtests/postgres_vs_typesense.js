import http from 'k6/http';
import { check, sleep } from 'k6';
import { Trend } from 'k6/metrics';

export let options = {
    stages: [
        { duration: '30s', target: 10 },  // ramp-up to 10 users
        { duration: '1m', target: 10 },   // sustain 10 users
        { duration: '30s', target: 0 },   // ramp-down
    ],
};

const queries = ['brush', 'toothpaste', 'shampoo', 'laptop', 'minecraft', 'shrek'];
const categories = [
    '0199708c-4eb6-71d1-9e48-83102c0155a4',
    null
];
const pages = [1, 2, 3, 4, 5];

export default function () {
    // Pick random query, category, and page for each request
    const q = queries[Math.floor(Math.random() * queries.length)];
    const category = categories[Math.floor(Math.random() * categories.length)];
    const page = pages[Math.floor(Math.random() * pages.length)];
    const per_page = 12;

    let url = `http://0.0.0.0:5077/api/productservice/v1/Product?q=${q}&page=${page}&per_page=${per_page}`;
    if (category) {
        const filter = encodeURIComponent(JSON.stringify({ CategoryId: [category] }));
        url += `&filter_by=${filter}`;
    }

    let res = http.get(url, {
        headers: { 'accept': 'text/plain' }
    });

    check(res, {
        'status is 200': (r) => r.status === 200,
        'response not empty': (r) => r.body.length > 0,
    });

    sleep(1);
}
