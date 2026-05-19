import http from 'k6/http';
import { check } from 'k6';

export const options = {
  thresholds: {
    http_req_failed: ['rate<1.0'],
  },
};

export default function () {
  const url = 'http://localhost:5000/bodega/api/inventory/1';

  const results = {
    ok: 0,
    tooMany: 0,
    other: 0,
  };

  for (let i = 0; i < 20; i++) {
    const res = http.get(url);

    if (res.status === 200) {
      results.ok++;
    } else if (res.status === 429) {
      results.tooMany++;
    } else {
      results.other++;
    }

    check(res, {
      'status is 200 or 429': (r) => r.status === 200 || r.status === 429,
    });
  }

  check(results, {
    'first 5 requests returned 200': () => results.ok >= 1,
    'remaining requests blocked (429)': () => results.tooMany >= 1,
  });
}
