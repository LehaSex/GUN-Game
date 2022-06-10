
/**
 * Main function
 */
const InitModule: nkruntime.InitModule =
        function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer) {
    // Hook
    initializer.registerAfterAuthenticateDevice(afterAuthenticateDeviceFn);
    // RPC
    initializer.registerRpc('add_user_coins', rpcAddUserCoins);
    logger.warn('Typescript loaded.');
}

const afterAuthenticateDeviceFn: nkruntime.AfterHookFunction<nkruntime.Session, nkruntime.AuthenticateDeviceRequest> =
        function(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, data: nkruntime.Session, req: nkruntime.AuthenticateDeviceRequest) {
    afterAuthenticate(ctx, logger, nk, data);
}


/**
 * After 1st auth
 */
function afterAuthenticate(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, data: nkruntime.Session) {
    if (!data.created) {
        // Account exists
        return
    }

/**    const initialState = {
 *       'level': Math.floor(Math.random() * 100),
 *       'wins': Math.floor(Math.random() * 100),
 *       'gamesPlayed': Math.floor(Math.random() * 200),
 *   }
 *
 *   const writeStats: nkruntime.StorageWriteRequest = {
 *       collection: 'stats',
 *       key: 'public',
 *       permissionRead: 2,
 *       permissionWrite: 0,
 *       value: initialState,
 *       userId: ctx.userId,
 *   }
 *
 *   try {
 *       nk.storageWrite([writeStats]);
 *   } catch (error) {
 *       logger.error('storageWrite error: %q', error);
 *       throw error;
 *   }
 */
    logger.debug('new user id: %s account data initialised', ctx.userId);
}

