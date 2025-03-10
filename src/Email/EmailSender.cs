using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace BackendKit;

internal sealed class EmailSender(EmailSettings settings)
{
  internal async Task Send(
    IEnumerable<string>? to,
    IEnumerable<string>? cc,
    IEnumerable<string>? bcc,
    string? subject,
    string? body,
    CancellationToken cancellation = default
  )
  {
    var email = new MimeMessage();

    email.From.Add(new MailboxAddress(settings.Name, settings.Address));

    if (to is not null)
    {
      foreach (var recipient in to)
      {
        email.To.Add(new MailboxAddress(recipient, recipient));
      }
    }

    if (cc is not null)
    {
      foreach (var recipient in cc)
      {
        email.Cc.Add(new MailboxAddress(recipient, recipient));
      }
    }

    if (bcc is not null)
    {
      foreach (var recipient in bcc)
      {
        email.Bcc.Add(new MailboxAddress(recipient, recipient));
      }
    }

    if (subject is not null)
    {
      email.Subject = subject;
    }

    if (body is not null)
    {
      email.Body = new TextPart(TextFormat.Html) { Text = body };
    }

    await _client.ConnectAsync(
      settings.Server,
      settings.Port,
      true,
      cancellation
    );
    await _client.AuthenticateAsync(
      settings.User,
      settings.Password,
      cancellation
    );
    await _client.SendAsync(email, cancellation);
    await _client.DisconnectAsync(true, cancellation);
  }

  private readonly SmtpClient _client = new();
}

internal sealed record EmailSettings(
  string Server,
  int Port,
  string User,
  string Password,
  string Name,
  string Address
);
