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

// const BASE_URL = 'http://192.168.49.2'; // k8s
const BASE_URL = 'http://0.0.0.0:5077'; // docker compose

export default function () {
    http.get(BASE_URL + '/api/productservice/v1/Category', {
        headers: { 'Content-Type': 'application/json' }
    });
    http.get(BASE_URL + '/api/cartservice/v1/Cart/dae5473d-2616-49d2-b169-c458de7fb0c6', {
        headers: { 'Content-Type': 'application/json' }
    });
    http.get(BASE_URL + '/api/orderservice/v1/Order?page=1&per_page=15', {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImRhZTU0NzNkLTI2MTYtNDlkMi1iMTY5LWM0NThkZTdmYjBjNiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGdtYWlsLmNvbSIsInVzZXIiOiJ7XCJJZFwiOlwiZGFlNTQ3M2QtMjYxNi00OWQyLWIxNjktYzQ1OGRlN2ZiMGM2XCIsXCJFbWFpbFwiOlwiYWRtaW5AZ21haWwuY29tXCIsXCJSb2xlc1wiOltdfSIsImV4cCI6MzkwNzIxMDU5MSwiaXNzIjoieW91ckNvbXBhbnlJc3N1ZXIuY29tIiwiYXVkIjoieW91ckNvbXBhbnlBdWRpZW5jZS5jb20ifQ.XGAJDfiLjLZjRlNCUO7ylLwaykveBkNPZkzrwxOjN_E'
        }
    });
    let data = { email: 'admin@gmail.com', password: 'password' };
    http.post(BASE_URL + '/api/userservice/Auth/Login', JSON.stringify(data), {
        headers: { 'Content-Type': 'application/json' }
    });

    sleep(1);
}

// docker compose
// avg=136.82ms min=316.53µs med=11.83ms max=1.07s p(90)=604.66ms p(95)=792.38ms

// k8s 1st run (pods are busy upsizing)
// avg=4.56s min=40.47ms med=3.69s max=9.81s  p(90)=8.56s p(95)=9.11s
// avg=3.81s min=40.17ms med=3.04s max=8.5s   p(90)=6.89s p(95)=7.37s
// k8s 2nd run (when pods are already upscaled by 1st run)
// avg=48.73ms min=40.44ms med=47.61ms max=79.95ms p(90)=54.63ms p(95)=57.8ms
// avg=49.49ms min=40.63ms med=48.46ms max=83.18ms p(90)=55.61ms p(95)=58.7ms