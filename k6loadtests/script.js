import http from 'k6/http'
import { check, sleep } from 'k6'

export const options = {
    iterations: 1000
}

export default function () {
    // const data = { username: 'username', password: 'password' }
    let res = http.get('http://localhost:5077/api/productservice')

    check(res, { 'success login': (r) => r.status === 200 })
}
