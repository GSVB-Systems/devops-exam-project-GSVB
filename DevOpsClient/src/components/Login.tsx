import { useState } from 'react'
import { useLogin } from '../hooks/useLogin'

type LoginProps = {
  onLoginSuccess?: (token: string) => void
}

const Login = ({ onLoginSuccess }: LoginProps) => {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const { login, isLoading, error, token, expiresInSeconds } = useLogin()

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    const trimmedEmail = email.trim()
    if (!trimmedEmail || !password) {
      return
    }

    const result = await login({ email: trimmedEmail, password })
    if (result?.accessToken) {
      onLoginSuccess?.(result.accessToken)
    }
  }

  return (
    <div className="card">
      <h2>Login</h2>
      <form onSubmit={handleSubmit}>
        <label htmlFor="loginEmail">Email</label>
        <input
          id="loginEmail"
          type="email"
          value={email}
          onChange={(event) => setEmail(event.target.value)}
          placeholder="you@example.com"
          autoComplete="username"
        />
        <label htmlFor="loginPassword">Password</label>
        <input
          id="loginPassword"
          type="password"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          placeholder="Enter your password"
          autoComplete="current-password"
        />
        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Logging in...' : 'Login'}
        </button>
      </form>
      {error ? <p className="error">{error}</p> : null}
      {token ? (
        <p className="result">
          Logged in. Token expires in {expiresInSeconds ?? 0} seconds.
        </p>
      ) : null}
    </div>
  )
}

export default Login

