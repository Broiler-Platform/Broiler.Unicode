import assert from 'node:assert/strict';
import test from 'node:test';
import { chooseVersion, readVersions, resolveVersion } from './resolve-preview-version.mjs';

test('first publish uses the configured preview; later publishes increment numerically', () => {
  assert.equal(chooseVersion('0.1.0-preview.1', []), '0.1.0-preview.1');
  assert.equal(chooseVersion('0.1.0-preview.1', ['0.1.0-preview.1']), '0.1.0-preview.2');
  assert.equal(chooseVersion('0.1.0-preview.1', ['0.1.0-preview.9', '0.1.0-preview.10']), '0.1.0-preview.11');
});

test('configured preview is a floor and other release lines do not affect it', () => {
  assert.equal(chooseVersion('0.1.0-preview.4', [
    '0.1.0-preview.1', '0.2.0-preview.99', '0.1.0', '0.1.0-rc.9',
  ]), '0.1.0-preview.4');
});

test('only unused previews on the configured release line are accepted', () => {
  const published = ['0.1.0-preview.1'];
  assert.equal(chooseVersion('0.1.0-preview.1', published, { suffix: 'preview.3' }), '0.1.0-preview.3');
  assert.equal(chooseVersion('0.1.0-preview.1', published, { tag: 'v0.1.0-preview.2' }), '0.1.0-preview.2');
  for (const suffix of ['preview.1', 'rc.2', 'preview.0', 'preview.02', 'preview.2;evil']) {
    assert.throws(() => chooseVersion('0.1.0-preview.1', published, { suffix }));
  }
  for (const tag of ['v0.1.0', 'v0.1.0-rc.2', 'v0.2.0-preview.2', 'v0.1.0-preview.1']) {
    assert.throws(() => chooseVersion('0.1.0-preview.1', published, { tag }));
  }
  assert.throws(() => chooseVersion('0.1.0', published));
});

function fakeFeed(responses) {
  return async (url, options) => {
    assert.ok(options.signal);
    if (url === 'https://feed/index.json') return Response.json({
      resources: [{ '@type': 'PackageBaseAddress/3.0.0', '@id': 'https://feed/flat/' }],
    });
    assert.ok(Object.hasOwn(responses, url), `Unexpected request ${url}`);
    const response = responses[url];
    return typeof response === 'number' ? new Response(null, { status: response }) : Response.json(response);
  };
}

test('all packages contribute, including a partially published newer preview', async () => {
  const versions = await readVersions('https://feed/index.json', ['Core', 'Provider', 'New'], {}, fakeFeed({
    'https://feed/flat/core/index.json': { versions: ['0.1.0-preview.1'] },
    'https://feed/flat/provider/index.json': { versions: ['0.1.0-preview.1', '0.1.0-preview.2'] },
    'https://feed/flat/new/index.json': 404,
  }));
  assert.equal(chooseVersion('0.1.0-preview.1', versions), '0.1.0-preview.3');
});

test('feed failures and malformed responses stop publication', async () => {
  for (const response of [401, 403, 429, 500, {}, { versions: [2] }]) {
    await assert.rejects(readVersions('https://feed/index.json', ['Core'], {}, fakeFeed({
      'https://feed/flat/core/index.json': response,
    })));
  }
  await assert.rejects(readVersions('https://feed/index.json', ['Core'], {}, async () => {
    throw new Error('Network unavailable');
  }));
  await assert.rejects(readVersions('https://feed/index.json', ['Core'], {}, async () => Response.json({})));
});

const packages = ['Core', 'Provider'].map(PackageId => ({ PackageId, PackageVersion: '0.1.0-preview.1' }));
const githubEnv = {
  GITHUB_REPOSITORY_OWNER: 'owner', GITHUB_ACTOR: 'actor', GITHUB_TOKEN: 'test-token',
};

function bothFeeds(nuget, github, calls = []) {
  return async (url, { headers, signal }) => {
    assert.ok(signal);
    calls.push(url);
    const isGithub = url.startsWith('https://nuget.pkg.github.com/owner/');
    if (isGithub) {
      assert.equal(headers.authorization, `Basic ${Buffer.from('actor:test-token').toString('base64')}`);
    } else {
      assert.ok(url.startsWith('https://api.nuget.org/'));
      assert.equal(headers.authorization, undefined, 'GitHub credentials must not be sent to nuget.org');
    }
    const base = isGithub ? 'https://nuget.pkg.github.com/owner/' : 'https://api.nuget.org/v3/';
    if (url === `${base}index.json`) return Response.json({
      resources: [{ '@type': 'PackageBaseAddress/3.0.0', '@id': `${base}flat/` }],
    });
    const id = url.slice(`${base}flat/`.length).replace('/index.json', '');
    const responses = isGithub ? github : nuget;
    assert.ok(Object.hasOwn(responses, id), `Unexpected request ${url}`);
    const response = responses[id];
    return typeof response === 'number' ? new Response(null, { status: response }) : Response.json(response);
  };
}

for (const target of ['nuget', 'github']) {
  test(`${target}: highest preview across both feeds and all packages determines the next version`, async () => {
    for (const [nugetNumber, githubNumber] of [[2, 3], [3, 2], [9, 10], [10, 9]]) {
      const calls = [];
      const version = await resolveVersion(packages, { ...githubEnv, TARGET: target }, bothFeeds(
        { core: { versions: [`0.1.0-preview.${nugetNumber}`] }, provider: 404 },
        { core: 404, provider: { versions: [`0.1.0-preview.${githubNumber}`] } }, calls,
      ));
      assert.equal(version, `0.1.0-preview.${Math.max(nugetNumber, githubNumber) + 1}`);
      assert.equal(calls.length, 6, 'Both indexes and every package on both feeds must be read');
    }
  });

  test(`${target}: new packages use the configured floor when absent from both feeds`, async () => {
    assert.equal(await resolveVersion(packages, { ...githubEnv, TARGET: target }, bothFeeds(
      { core: 404, provider: 404 }, { core: 404, provider: 404 },
    )), '0.1.0-preview.1');
  });

  test(`${target}: suffixes and tags cannot reuse a preview from either feed`, async () => {
    for (const [nugetNumber, githubNumber] of [[2, 3], [3, 2]]) {
      const fetchImpl = bothFeeds(
        { core: { versions: [`0.1.0-preview.${nugetNumber}`] }, provider: 404 },
        { core: { versions: [`0.1.0-preview.${githubNumber}`] }, provider: 404 },
      );
      for (const override of [
        { VERSION_SUFFIX: 'preview.3' },
        { GITHUB_EVENT_NAME: 'push', GITHUB_REF: 'refs/tags/v0.1.0-preview.3' },
      ]) {
        await assert.rejects(resolveVersion(packages, { ...githubEnv, TARGET: target, ...override }, fetchImpl),
          /at least '0.1.0-preview.4'/);
      }
      for (const override of [
        { VERSION_SUFFIX: 'preview.4' },
        { GITHUB_EVENT_NAME: 'push', GITHUB_REF: 'refs/tags/v0.1.0-preview.4' },
      ]) {
        assert.equal(await resolveVersion(packages, { ...githubEnv, TARGET: target, ...override }, fetchImpl),
          '0.1.0-preview.4');
      }
    }
  });

  test(`${target}: neither feed may fail silently`, async () => {
    for (const failure of [401, 403, 429, 500, {}]) {
      const good = { core: { versions: ['0.1.0-preview.2'] }, provider: 404 };
      const bad = { core: failure, provider: 404 };
      for (const feeds of [[good, bad], [bad, good]]) {
        await assert.rejects(resolveVersion(packages, { ...githubEnv, TARGET: target }, bothFeeds(...feeds)));
      }
    }
    for (const missing of Object.keys(githubEnv)) {
      await assert.rejects(resolveVersion(packages, { ...githubEnv, TARGET: target, [missing]: '' }, () => {
        assert.fail('Credentials must be checked before accessing either feed');
      }), /GitHub feed lookup requires/);
    }
  });
}
