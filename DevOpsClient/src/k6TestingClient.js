import http from 'k6/http';

export const options = {
    stages: [
        { duration: '30s', target: 50 }, // Ramp up to 10 users over 30 seconds
        { duration: '1m', target: 25 },  // Stay at 10 users for 1 minute
        { duration: '30s', target: 0 },   // Ramp down to 0 users over 30 seconds
    ],
};

export default () => {
    http.get('https://devopsclient.fly.dev')
};