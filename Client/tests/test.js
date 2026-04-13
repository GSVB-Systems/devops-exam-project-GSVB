import { ClientFunction, RequestMock } from 'testcafe'
import page from './page-model.js'

const baseUrl = process.env.E2E_BASE_URL ?? 'http://localhost:5173'

const locationPathname = ClientFunction(() => window.location.pathname)

const userProfileResponse = {
  userId: 'user-1',
  username: 'Test Farmer',
  email: 'user@example.com'
}

const eggAccountsResponse = [
  {
    id: 'account-1',
    eiUserId: 'ei_1',
    status: 'Main',
    userName: 'Test Coop',
    lastFetchedUtc: '2026-01-01T00:00:00.000Z'
  }
]

const loginRequestMock = RequestMock()
  .onRequestTo({ method: 'post', url: /\/api\/User\/login$/i })
  .respond(
    {
      accessToken: 'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1c2VyLTEifQ.signature',
      tokenType: 'Bearer',
      expiresInSeconds: 3600
    },
    200,
    { 'access-control-allow-origin': '*' }
  )

const userAndAccountsRequestMock = RequestMock()
  .onRequestTo({ method: 'get', url: /\/api\/User\/me$/i })
  .respond(userProfileResponse, 200, { 'access-control-allow-origin': '*' })
  .onRequestTo({ method: 'get', url: /\/api\/egg-accounts$/i })
  .respond(eggAccountsResponse, 200, { 'access-control-allow-origin': '*' })

fixture`EggBoard Client E2E`
  .page(`${baseUrl}/login`)
  .requestHooks(loginRequestMock, userAndAccountsRequestMock)
  .beforeEach(async (t) => {
    await t.eval(() => {
      localStorage.clear()
      sessionStorage.clear()
    })
  })

test('redirects unauthenticated users from root to login', async (t) => {
  await t.navigateTo(`${baseUrl}/`)

  await t.expect(page.loginTitle.exists).ok()
  await t.expect(locationPathname()).eql('/login')
})

test('shows validation error when register passwords do not match', async (t) => {
  await t.navigateTo(`${baseUrl}/register`)

  await t
    .typeText(page.registerUsername, 'new-user')
    .typeText(page.registerEmail, 'new-user@example.com')
    .typeText(page.registerPassword, 'Password123!')
    .typeText(page.registerPasswordConfirm, 'Password123!x')
    .click(page.registerSubmitButton)

  await t.expect(page.passwordMismatchError.exists).ok()
})

test('logs in and lands on dashboard', async (t) => {
  await t.navigateTo(`${baseUrl}/login`)

  await t
    .typeText(page.loginEmail, 'user@example.com')
    .typeText(page.loginPassword, 'Password123!')
    .click(page.loginButton)

  await t.expect(locationPathname()).eql('/Dashboard')
  await t.expect(page.refreshSnapshotButton.exists).ok()
})
