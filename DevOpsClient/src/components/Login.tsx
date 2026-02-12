import { useState } from 'react'
import { type Location, useLocation, useNavigate } from 'react-router-dom'
import { useLogin } from '../hooks/useLogin'

type LoginProps = {
  onLoginSuccess?: (token: string) => void
}

const Login = ({ onLoginSuccess }: LoginProps) => {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const { login, isLoading, error, token, expiresInSeconds } = useLogin()
  const navigate = useNavigate()
  const location = useLocation()
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
    <div className="card w-full max-w-md mx-auto bg-base-100 shadow-md">
      <div className="card-body">
        <h2 className="card-title">Login</h2>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="loginEmail" className="label">
              <span className="label-text">Email</span>
            </label>
            <input
              id="loginEmail"
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="you@example.com"
              autoComplete="username"
              className="input input-bordered w-full"
            />
          </div>

          <div>
            <label htmlFor="loginPassword" className="label">
              <span className="label-text">Password</span>
            </label>
            <input
              id="loginPassword"
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              placeholder="Enter your password"
              autoComplete="current-password"
              className="input input-bordered w-full"
            />
          </div>

          <div className="flex justify-end">
            <button type="submit" className={`btn btn-primary ${isLoading ? 'loading' : ''}`} disabled={isLoading}>
              {isLoading ? 'Logging in...' : 'Login'}
            </button>
          </div>
        </form>

        {error ? <p className="text-error mt-2">{error}</p> : null}

        {token ? (
          <p className="text-success mt-2">
            Logged in. Token expires in {expiresInSeconds ?? 0} seconds.
          </p>
        ) : null}
      </div>
    </div>
  )
}

export default Login
