import { Selector } from 'testcafe'

class Page {
  loginTitle = Selector('h2').withText('EggBoard')
  loginEmail = Selector('#loginEmail')
  loginPassword = Selector('#loginPassword')
  loginButton = Selector('button').withText('Login')

  registerUsername = Selector('#registerUsername')
  registerDiscordUsername = Selector('#registerDiscordUsername')
  registerEmail = Selector('#registerEmail')
  registerPassword = Selector('#registerPassword')
  registerPasswordConfirm = Selector('#registerPasswordConfirm')
  registerSubmitButton = Selector('button').withText('Create Account')

  passwordMismatchError = Selector('p').withText('Passwords do not match.')
  refreshSnapshotButton = Selector('button').withText('Refresh Snapshot')
}

export default new Page()
