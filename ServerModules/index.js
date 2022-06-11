"use strict";
var currencyKeyName = 'coins';
var rpcAddUserGems = function (ctx, logger, nk) {
    var walletUpdateResult = updateWallet(nk, ctx.userId, 100, {});
    var updateString = JSON.stringify(walletUpdateResult);
    logger.debug('Added 100 coins to user %s wallet: %s', ctx.userId, updateString);
    return updateString;
};
function updateWallet(nk, userId, amount, metadata) {
    var _a;
    var changeset = (_a = {},
        _a[currencyKeyName] = amount,
        _a);
    var result = nk.walletUpdate(userId, changeset, metadata, true);
    return result;
}
var InitModule = function (ctx, logger, nk, initializer) {
    initializer.registerAfterAuthenticateDevice(afterAuthenticateDeviceFn);
    initializer.registerRpc('search_username', rpcSearchUsernameFn);
    initializer.registerRpc('add_user_gems', rpcAddUserGems);
    logger.warn('Typescript loaded.');
};
var afterAuthenticateDeviceFn = function (ctx, logger, nk, data, req) {
    afterAuthenticate(ctx, logger, nk, data);
};
function afterAuthenticate(ctx, logger, nk, data) {
    if (!data.created) {
        return;
    }
    var initialState = {
        'level': Math.floor(Math.random() * 100),
        'wins': Math.floor(Math.random() * 100),
        'gamesPlayed': Math.floor(Math.random() * 200),
    };
    var writeStats = {
        collection: 'stats',
        key: 'public',
        permissionRead: 2,
        permissionWrite: 0,
        value: initialState,
        userId: ctx.userId,
    };
    try {
        nk.storageWrite([writeStats]);
    }
    catch (error) {
        logger.error('storageWrite error: %q', error);
        throw error;
    }
    logger.debug('new user id: %s account data initialised', ctx.userId);
}
var rpcSearchUsernameFn = function (ctx, logger, nk, payload) {
    var input = JSON.parse(payload);
    var query = "\n    SELECT id, username FROM users WHERE username ILIKE concat($1, '%')\n    ";
    var result = nk.sqlQuery(query, [input.username]);
    return JSON.stringify(result);
};
