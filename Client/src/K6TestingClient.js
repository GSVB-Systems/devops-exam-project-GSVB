import http from 'k6/http';
import { check } from 'k6';

export const options = {
    stages: [
        { duration: '30s', target: 50 }, // Ramp up to 10 users over 30 seconds
        { duration: '1m', target: 25 },  // Stay at 10 users for 1 minute
        { duration: '30s', target: 0 },   // Ramp down to 0 users over 30 seconds
    ],
    thresholds: {
        'http_req_failed{name:client-home}': ['rate<0.05'],
        'http_req_duration{name:client-home}': ['p(50)<15000', 'p(99)<25000'],
        'checks{name:client-home}': ['rate>0.95'],
    },
};

export default () => {
    const res = http.get('https://devopsclient.fly.dev', {
        tags: { name: 'client-home' },
    });
    check(res, { 'client home returns 200': (r) => r.status === 200 }, { name: 'client-home' });
};