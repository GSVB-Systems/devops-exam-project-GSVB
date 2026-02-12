import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { userApi } from '../api/userApi'

const RegisterPage = () => {
  const [username, setUsername] = useState('')
  const [discordUsername, setDiscordUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  const navigate = useNavigate()

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    if (!username.trim() || !email.trim() || !password) {
      setError('Username, email, and password are required.')
      return
    }

    if (password !== confirmPassword) {
      setError('Passwords do not match.')
      return
    }

    setIsSubmitting(true)
    setError(null)

    try {
      await userApi.register({
        username: username.trim(),
        discordUsername: discordUsername.trim() ? discordUsername.trim() : null,
        email: email.trim(),
        password
      })
      setSuccess('Account created. You can now log in.')
      setTimeout(() => navigate('/login'), 800)
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to create account.'
      setError(message)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="shell-panel login-shell rise-in">
      <p className="eyebrow">Register</p>
      <h2 className="panel-title">Create your EggBoard account</h2>
      <p className="text-sm opacity-70">Link your Egg accounts after signing in.</p>

      <form onSubmit={handleSubmit} className="space-y-4 mt-6">
        <div>
          <label htmlFor="registerUsername" className="field-label">
            Username
          </label>
          <input
            id="registerUsername"
            type="text"
            value={username}
            onChange={(event) => setUsername(event.target.value)}
            className="input input-bordered w-full shell-input"
            placeholder="Farmer name"
          />
        </div>
        
        <div>
          <label htmlFor="registerEmail" className="field-label">
            Email
          </label>
          <input
            id="registerEmail"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            className="input input-bordered w-full shell-input"
            placeholder="you@example.com"
          />
        </div>

        <div>
          <label htmlFor="registerPassword" className="field-label">
            Password
          </label>
          <input
            id="registerPassword"
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            className="input input-bordered w-full shell-input"
            placeholder="Create a password"
          />
        </div>

        <div>
          <label htmlFor="registerPasswordConfirm" className="field-label">
            Confirm Password
          </label>
          <input
            id="registerPasswordConfirm"
            type="password"
            value={confirmPassword}
            onChange={(event) => setConfirmPassword(event.target.value)}
            className="input input-bordered w-full shell-input"
            placeholder="Repeat password"
          />
        </div>

        <div className="flex justify-end">
          <button type="submit" className={`btn btn-primary ${isSubmitting ? 'loading' : ''}`} disabled={isSubmitting}>
            {isSubmitting ? 'Creating...' : 'Create Account'}
          </button>
        </div>
      </form>

      {error ? <p className="text-error mt-4">{error}</p> : null}
      {success ? <p className="text-success mt-4">{success}</p> : null}
    </div>
  )
}

export default RegisterPage
