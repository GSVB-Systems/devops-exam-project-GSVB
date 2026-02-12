import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useLogin } from '../hooks/useLogin'

type LoginProps = {
  onLoginSuccess?: (token: string) => void
}

const Login = ({ onLoginSuccess }: LoginProps) => {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const { login, isLoading, error, token, expiresInSeconds } = useLogin()
  const navigate = useNavigate()
  const redirectPath = '/'

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    const trimmedEmail = email.trim()
    if (!trimmedEmail || !password) {
      return
    }

    const result = await login({ email: trimmedEmail, password })
    if (result?.accessToken) {
      onLoginSuccess?.(result.accessToken)
      navigate(redirectPath, { replace: true })
    }
  }

  return (
    <div className="shell-panel login-shell rise-in">
      <h2 className="panel-title">EggBoard</h2>

      <form onSubmit={handleSubmit} className="space-y-4 mt-6">
        <div>
          <label htmlFor="loginEmail" className="field-label">
            Email
          </label>
          <input
            id="loginEmail"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            placeholder="you@example.com"
            autoComplete="username"
            className="input input-bordered w-full shell-input"
          />
        </div>

        <div>
          <label htmlFor="loginPassword" className="field-label">
            Password
          </label>
          <input
            id="loginPassword"
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            placeholder="Enter your password"
            autoComplete="current-password"
            className="input input-bordered w-full shell-input"
          />
        </div>

        <div className="flex justify-end">
          <button type="submit" className={`btn btn-primary ${isLoading ? 'loading' : ''}`} disabled={isLoading}>
            {isLoading ? 'Logging in...' : 'Login'}
          </button>
        </div>
      </form>

      {error ? <p className="text-error mt-4">{error}</p> : null}

      <div className="mt-6 text-sm opacity-70">
        New here? <Link to="/register" className="link">Create an account</Link>
      </div>

      {token ? (
        <p className="text-success mt-2">
          Logged in. Token expires in {expiresInSeconds ?? 0} seconds.
        </p>
      ) : null}
    </div>
  )
}

export default Login
