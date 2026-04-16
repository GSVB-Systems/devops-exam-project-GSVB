import http from 'k6/http';
import { check } from 'k6';

export const options = {
   stages: [
       { duration: '10s', target: 50 }, // Ramp up to 10 users over 30 seconds
       { duration: '20s', target: 25 }, // Stay at 10 users for 1 minute
       { duration: '30s', target: 0 }, // Ramp down to 0 users over 30 seconds
   ],
   thresholds: {
       'http_req_failed{name:login}': ['rate<0.05'],
       'http_req_duration{name:login}': ['p(95)<750', 'p(99)<1500'],
       'checks{name:login}': ['rate>0.95'],
   },
};

const credentials = {
    email: __ENV.K6LOGINTEST_EMAIL,
    password: __ENV.K6LOGINTEST_PASSWORD,
};


const payload = JSON.stringify({
    email: credentials.email,
    password: credentials.password,
});

const params = {
    headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
    },
    tags: { name: 'login' },
};

export default () => {
   const res = http.post('https://server-spring-cloud-8981.fly.dev/api/User/login', payload, params);
   check(res, { 'login returns 200': (r) => r.status === 200 }, { name: 'login' });
};