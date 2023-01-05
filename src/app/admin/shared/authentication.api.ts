export abstract class AuthenticationApi {
  abstract get name(): string | null;

  abstract get eMail(): string | null;

  abstract gravatarURL(size: number): string | null;
}
