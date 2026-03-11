import http from 'k6/http';

export const options = {
   stages: [
       { duration: '10s', target: 50 }, // Ramp up to 10 users over 30 seconds
       { duration: '20s', target: 25 }, // Stay at 10 users for 1 minute
       { duration: '30s', target: 0 }, // Ramp down to 0 users over 30 seconds
   ],
};

const credentials = {
    email: __ENV.EMAIL,
    password: __ENV.PASSWORD,
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
};
export default () => {
   http.post('https://server-spring-cloud-8981.fly.dev/api/User/login', payload, params);
};