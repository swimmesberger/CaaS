export abstract class AuthenticationApi {
  abstract get name(): string | null;

  abstract get eMail(): string | null;

  abstract logout(): void;

  abstract gravatarURL(size: number): string | null;
}
