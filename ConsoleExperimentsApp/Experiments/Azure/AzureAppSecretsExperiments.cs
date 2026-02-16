//using System;
//using System.Collections.Generic;
//using System.Text;
//using Azure;
//using Azure.Identity;
//using Azure.Security.KeyVault.Secrets;
//using Azure.Data.AppConfiguration;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.AzureAppConfiguration;

//namespace ConsoleExperimentsApp.Experiments.Azure
//{
//    public static class AzureAppSecretsExperiments
//    {
//        public static async Task Run()
//        {
//            Console.ForegroundColor = ConsoleColor.Cyan;
//            Console.WriteLine("=== Azure App Secrets Experiments ===");
//            Console.WriteLine("Description: Comprehensive examples of Azure Key Vault and App Configuration");
//            Console.ResetColor();

//            Console.WriteLine("\n========== AZURE KEY VAULT EXPERIMENTS ==========\n");
//            await RunKeyVaultExperiments();

//            Console.WriteLine("\n\n========== AZURE APP CONFIGURATION EXPERIMENTS ==========\n");
//            await RunAppConfigurationExperiments();

//            Console.ForegroundColor = ConsoleColor.Magenta;
//            Console.WriteLine("\nPress Enter to exit...");
//            Console.ResetColor();
//        }

//        #region Azure Key Vault Experiments

//        private static async Task RunKeyVaultExperiments()
//        {
//            // NOTE: Set your Key Vault name or URL
//            // You can get this from Azure Portal -> Key Vault -> Overview
//            string keyVaultName = Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_NAME") ?? "your-keyvault-name";
//            string keyVaultUrl = $"https://{keyVaultName}.vault.azure.net/";

//            // Using DefaultAzureCredential which works with:
//            // - Azure CLI (az login)
//            // - Managed Identity (when deployed to Azure)
//            // - Visual Studio
//            // - Environment variables
//            var credential = new DefaultAzureCredential();
//            var client = new SecretClient(new Uri(keyVaultUrl), credential);

//            try
//            {
//                Console.WriteLine("1. Creating and Setting Secrets");
//                await CreateSecretsExample(client);

//                Console.WriteLine("\n2. Reading Secrets");
//                await ReadSecretsExample(client);

//                Console.WriteLine("\n3. Updating Secrets");
//                await UpdateSecretsExample(client);

//                Console.WriteLine("\n4. Listing Secrets");
//                await ListSecretsExample(client);

//                Console.WriteLine("\n5. Working with Secret Versions");
//                await SecretVersionsExample(client);

//                Console.WriteLine("\n6. Secret Properties and Metadata");
//                await SecretPropertiesExample(client);

//                Console.WriteLine("\n7. Enabling and Disabling Secrets");
//                await EnableDisableSecretsExample(client);

//                Console.WriteLine("\n8. Secret Expiration and Activation");
//                await SecretExpirationExample(client);

//                Console.WriteLine("\n9. Deleting and Recovering Secrets");
//                await DeleteRecoverSecretsExample(client);

//                Console.WriteLine("\n10. Bulk Operations");
//                await BulkOperationsExample(client);
//            }
//            catch (AuthenticationFailedException ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"❌ Authentication failed: {ex.Message}");
//                Console.WriteLine("Make sure you're logged in with 'az login' or have proper credentials configured.");
//                Console.ResetColor();
//            }
//            catch (RequestFailedException ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"❌ Request failed: {ex.Message}");
//                Console.WriteLine($"Status: {ex.Status}, Error Code: {ex.ErrorCode}");
//                Console.ResetColor();
//            }
//            catch (Exception ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"❌ Error: {ex.Message}");
//                Console.ResetColor();
//            }
//        }

//        private static async Task CreateSecretsExample(SecretClient client)
//        {
//            try
//            {
//                // Create a simple secret
//                await client.SetSecretAsync("DatabasePassword", "MySecureP@ssw0rd123!");
//                Console.WriteLine("✓ Created secret: DatabasePassword");

//                // Create a connection string secret
//                await client.SetSecretAsync("SqlConnectionString", 
//                    "Server=myserver.database.windows.net;Database=mydb;User Id=admin;Password=pass123;");
//                Console.WriteLine("✓ Created secret: SqlConnectionString");

//                // Create an API key secret
//                await client.SetSecretAsync("ThirdPartyApiKey", "sk_test_51Hxxxxxxxxxxxxxx");
//                Console.WriteLine("✓ Created secret: ThirdPartyApiKey");

//                // Create a certificate secret (base64 encoded)
//                await client.SetSecretAsync("CertificateData", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Certificate content here")));
//                Console.WriteLine("✓ Created secret: CertificateData");
//            }
//            catch (RequestFailedException ex) when (ex.Status == 403)
//            {
//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("⚠ Access denied. Make sure you have 'Key Vault Secrets Officer' or 'Key Vault Administrator' role.");
//                Console.ResetColor();
//            }
//        }

//        private static async Task ReadSecretsExample(SecretClient client)
//        {
//            try
//            {
//                // Read a secret
//                KeyVaultSecret secret = await client.GetSecretAsync("DatabasePassword");
//                Console.WriteLine($"✓ Retrieved secret: {secret.Name}");
//                Console.WriteLine($"  Value: {MaskSecret(secret.Value)}");
//                Console.WriteLine($"  Created: {secret.Properties.CreatedOn}");
//                Console.WriteLine($"  Updated: {secret.Properties.UpdatedOn}");

//                // Read specific version
//                var allVersions = client.GetPropertiesOfSecretVersionsAsync("DatabasePassword");
//                await foreach (var version in allVersions)
//                {
//                    if (version.Version != null)
//                    {
//                        var versionedSecret = await client.GetSecretAsync("DatabasePassword", version.Version);
//                        Console.WriteLine($"  Version {version.Version}: {MaskSecret(versionedSecret.Value)}");
//                        break; // Just show one version as example
//                    }
//                }
//            }
//            catch (RequestFailedException ex) when (ex.Status == 404)
//            {
//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("⚠ Secret not found (this is expected if it wasn't created)");
//                Console.ResetColor();
//            }
//        }

//        private static async Task UpdateSecretsExample(SecretClient client)
//        {
//            try
//            {
//                // Update secret value (creates new version)
//                var updatedSecret = await client.SetSecretAsync("DatabasePassword", "NewSecureP@ssw0rd456!");
//                Console.WriteLine($"✓ Updated secret: {updatedSecret.Value.Name}");
//                Console.WriteLine($"  New version: {updatedSecret.Value.Properties.Version}");

//                // Update secret properties without changing value
//                var secret = await client.GetSecretAsync("DatabasePassword");
//                secret.Properties.ContentType = "password";
//                secret.Properties.Tags["Environment"] = "Production";
//                secret.Properties.Tags["Owner"] = "DevOps Team";

//                await client.UpdateSecretPropertiesAsync(secret.Properties);
//                Console.WriteLine("✓ Updated secret properties and tags");
//            }
//            catch (RequestFailedException ex) when (ex.Status == 404)
//            {
//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("⚠ Secret not found");
//                Console.ResetColor();
//            }
//        }

//        private static async Task ListSecretsExample(SecretClient client)
//        {
//            try
//            {
//                Console.WriteLine("Listing all secrets:");
//                var secrets = client.GetPropertiesOfSecretsAsync();
//                int count = 0;

//                await foreach (var secretProperties in secrets)
//                {
//                    count++;
//                    Console.WriteLine($"  - {secretProperties.Name}");
//                    Console.WriteLine($"    Enabled: {secretProperties.Enabled}");
//                    Console.WriteLine($"    Content Type: {secretProperties.ContentType ?? "N/A"}");

//                    if (secretProperties.Tags.Count > 0)
//                    {
//                        Console.WriteLine("    Tags:");
//                        foreach (var tag in secretProperties.Tags)
//                        {
//                            Console.WriteLine($"      {tag.Key}: {tag.Value}");
//                        }
//                    }

//                    if (count >= 5) break; // Limit output
//                }

//                Console.WriteLine($"✓ Listed {count} secret(s)");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error listing secrets: {ex.Message}");
//            }
//        }

//        private static async Task SecretVersionsExample(SecretClient client)
//        {
//            try
//            {
//                string secretName = "DatabasePassword";

//                // Create multiple versions
//                await client.SetSecretAsync(secretName, "Version1");
//                await Task.Delay(100);
//                await client.SetSecretAsync(secretName, "Version2");
//                await Task.Delay(100);
//                await client.SetSecretAsync(secretName, "Version3");

//                Console.WriteLine($"Versions of {secretName}:");
//                var versions = client.GetPropertiesOfSecretVersionsAsync(secretName);
//                int count = 0;

//                await foreach (var version in versions)
//                {
//                    count++;
//                    Console.WriteLine($"  Version: {version.Version}");
//                    Console.WriteLine($"    Created: {version.CreatedOn}");
//                    Console.WriteLine($"    Updated: {version.UpdatedOn}");
//                    Console.WriteLine($"    Enabled: {version.Enabled}");

//                    if (count >= 3) break; // Limit output
//                }

//                Console.WriteLine($"✓ Found {count} version(s)");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error working with versions: {ex.Message}");
//            }
//        }

//        private static async Task SecretPropertiesExample(SecretClient client)
//        {
//            try
//            {
//                string secretName = "SecretWithProperties";

//                // Create secret with properties
//                var secret = await client.SetSecretAsync(secretName, "MySecretValue");

//                // Update properties
//                var properties = secret.Value.Properties;
//                properties.ContentType = "application/json";
//                properties.Tags["Environment"] = "Development";
//                properties.Tags["Application"] = "ExperimentsApp";
//                properties.Tags["CostCenter"] = "Engineering";
//                properties.NotBefore = DateTimeOffset.UtcNow;
//                properties.ExpiresOn = DateTimeOffset.UtcNow.AddDays(90);

//                await client.UpdateSecretPropertiesAsync(properties);

//                // Retrieve and display
//                var retrievedSecret = await client.GetSecretAsync(secretName);
//                Console.WriteLine($"✓ Secret: {retrievedSecret.Value.Name}");
//                Console.WriteLine($"  Content Type: {retrievedSecret.Value.Properties.ContentType}");
//                Console.WriteLine($"  Not Before: {retrievedSecret.Value.Properties.NotBefore}");
//                Console.WriteLine($"  Expires On: {retrievedSecret.Value.Properties.ExpiresOn}");
//                Console.WriteLine("  Tags:");
//                foreach (var tag in retrievedSecret.Value.Properties.Tags)
//                {
//                    Console.WriteLine($"    {tag.Key}: {tag.Value}");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with secret properties: {ex.Message}");
//            }
//        }

//        private static async Task EnableDisableSecretsExample(SecretClient client)
//        {
//            try
//            {
//                string secretName = "ToggleableSecret";

//                // Create secret
//                await client.SetSecretAsync(secretName, "MyValue");
//                Console.WriteLine($"✓ Created secret: {secretName}");

//                // Disable secret
//                var secret = await client.GetSecretAsync(secretName);
//                secret.Properties.Enabled = false;
//                await client.UpdateSecretPropertiesAsync(secret.Properties);
//                Console.WriteLine($"✓ Disabled secret: {secretName}");

//                // Try to read disabled secret
//                var disabledSecret = await client.GetSecretAsync(secretName);
//                Console.WriteLine($"  Can still read disabled secret: {!disabledSecret.Value.Properties.Enabled ?? false}");

//                // Re-enable secret
//                disabledSecret.Properties.Enabled = true;
//                await client.UpdateSecretPropertiesAsync(disabledSecret.Properties);
//                Console.WriteLine($"✓ Re-enabled secret: {secretName}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error toggling secret: {ex.Message}");
//            }
//        }

//        private static async Task SecretExpirationExample(SecretClient client)
//        {
//            try
//            {
//                // Secret that expires in 30 days
//                var shortLivedSecret = await client.SetSecretAsync("ShortLivedSecret", "TemporaryValue");
//                shortLivedSecret.Value.Properties.ExpiresOn = DateTimeOffset.UtcNow.AddDays(30);
//                await client.UpdateSecretPropertiesAsync(shortLivedSecret.Value.Properties);
//                Console.WriteLine("✓ Created secret expiring in 30 days");

//                // Secret that's not active yet
//                var futureSecret = await client.SetSecretAsync("FutureSecret", "FutureValue");
//                futureSecret.Value.Properties.NotBefore = DateTimeOffset.UtcNow.AddDays(7);
//                futureSecret.Value.Properties.ExpiresOn = DateTimeOffset.UtcNow.AddDays(37);
//                await client.UpdateSecretPropertiesAsync(futureSecret.Value.Properties);
//                Console.WriteLine("✓ Created secret active in 7 days, expiring in 37 days");

//                // Check expiration status
//                var secrets = client.GetPropertiesOfSecretsAsync();
//                Console.WriteLine("\nSecret expiration status:");
//                int count = 0;
//                await foreach (var secretProps in secrets)
//                {
//                    if (secretProps.ExpiresOn.HasValue)
//                    {
//                        var daysUntilExpiry = (secretProps.ExpiresOn.Value - DateTimeOffset.UtcNow).Days;
//                        Console.WriteLine($"  {secretProps.Name}: Expires in {daysUntilExpiry} days");
//                        count++;
//                        if (count >= 3) break;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with secret expiration: {ex.Message}");
//            }
//        }

//        private static async Task DeleteRecoverSecretsExample(SecretClient client)
//        {
//            try
//            {
//                string secretName = "TemporarySecret";

//                // Create a secret
//                await client.SetSecretAsync(secretName, "TemporaryValue");
//                Console.WriteLine($"✓ Created secret: {secretName}");

//                // Delete the secret (soft delete)
//                var deleteOperation = await client.StartDeleteSecretAsync(secretName);
//                await deleteOperation.WaitForCompletionAsync();
//                Console.WriteLine($"✓ Soft-deleted secret: {secretName}");

//                // List deleted secrets
//                var deletedSecrets = client.GetDeletedSecretsAsync();
//                Console.WriteLine("Deleted secrets:");
//                int count = 0;
//                await foreach (var deletedSecret in deletedSecrets)
//                {
//                    Console.WriteLine($"  - {deletedSecret.Name}");
//                    Console.WriteLine($"    Deleted On: {deletedSecret.DeletedOn}");
//                    Console.WriteLine($"    Scheduled Purge: {deletedSecret.ScheduledPurgeDate}");
//                    count++;
//                    if (count >= 3) break;
//                }

//                // Recover the secret
//                var recoverOperation = await client.StartRecoverDeletedSecretAsync(secretName);
//                await recoverOperation.WaitForCompletionAsync();
//                Console.WriteLine($"✓ Recovered secret: {secretName}");

//                // Clean up - delete permanently
//                await client.StartDeleteSecretAsync(secretName);
//                Console.WriteLine($"✓ Cleaned up test secret");
//            }
//            catch (RequestFailedException ex) when (ex.Status == 404)
//            {
//                Console.WriteLine("⚠ Secret not found (may have been purged)");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with delete/recover: {ex.Message}");
//            }
//        }

//        private static async Task BulkOperationsExample(SecretClient client)
//        {
//            try
//            {
//                Console.WriteLine("Creating multiple secrets in bulk...");

//                var secretsToCreate = new Dictionary<string, string>
//                {
//                    { "BulkSecret1", "Value1" },
//                    { "BulkSecret2", "Value2" },
//                    { "BulkSecret3", "Value3" },
//                    { "BulkSecret4", "Value4" },
//                    { "BulkSecret5", "Value5" }
//                };

//                var tasks = secretsToCreate.Select(kvp => 
//                    client.SetSecretAsync(kvp.Key, kvp.Value)).ToList();

//                await Task.WhenAll(tasks);
//                Console.WriteLine($"✓ Created {tasks.Count} secrets in parallel");

//                // Bulk read
//                Console.WriteLine("\nReading secrets in bulk...");
//                var readTasks = secretsToCreate.Keys.Select(name => 
//                    client.GetSecretAsync(name)).ToList();

//                var results = await Task.WhenAll(readTasks);
//                Console.WriteLine($"✓ Read {results.Length} secrets in parallel");

//                // Clean up
//                var deleteTasks = secretsToCreate.Keys.Select(name => 
//                    client.StartDeleteSecretAsync(name)).ToList();
//                await Task.WhenAll(deleteTasks);
//                Console.WriteLine($"✓ Deleted {deleteTasks.Count} secrets");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with bulk operations: {ex.Message}");
//            }
//        }

//        #endregion

//        #region Azure App Configuration Experiments

//        private static async Task RunAppConfigurationExperiments()
//        {
//            // NOTE: Set your App Configuration connection string
//            // You can get this from Azure Portal -> App Configuration -> Access keys
//            string connectionString = Environment.GetEnvironmentVariable("AZURE_APPCONFIG_CONNECTION_STRING") 
//                                    ?? "Endpoint=https://your-appconfig.azconfig.io;Id=xxxxx;Secret=xxxxx";

//            try
//            {
//                var client = new ConfigurationClient(connectionString);

//                Console.WriteLine("1. Setting Configuration Values");
//                await SetConfigurationExample(client);

//                Console.WriteLine("\n2. Reading Configuration Values");
//                await ReadConfigurationExample(client);

//                Console.WriteLine("\n3. Using Labels for Environments");
//                await LabelsExample(client);

//                Console.WriteLine("\n4. Feature Flags");
//                await FeatureFlagsExample(client);

//                Console.WriteLine("\n5. Configuration with Content Type");
//                await ContentTypeExample(client);

//                Console.WriteLine("\n6. Key-Value Tags");
//                await TagsExample(client);

//                Console.WriteLine("\n7. Listing and Filtering Configuration");
//                await ListConfigurationExample(client);

//                Console.WriteLine("\n8. Configuration Snapshots");
//                await SnapshotsExample(client);

//                Console.WriteLine("\n9. Key Vault References");
//                await KeyVaultReferencesExample(client);

//                Console.WriteLine("\n10. Integration with IConfiguration");
//                await IConfigurationIntegrationExample(connectionString);
//            }
//            catch (RequestFailedException ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"❌ Request failed: {ex.Message}");
//                Console.WriteLine($"Status: {ex.Status}, Error Code: {ex.ErrorCode}");
//                Console.WriteLine("Make sure your connection string is valid.");
//                Console.ResetColor();
//            }
//            catch (Exception ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"❌ Error: {ex.Message}");
//                Console.ResetColor();
//            }
//        }

//        private static async Task SetConfigurationExample(ConfigurationClient client)
//        {
//            try
//            {
//                // Set simple configuration values
//                await client.SetConfigurationSettingAsync("AppSettings:MaxRetries", "3");
//                Console.WriteLine("✓ Set: AppSettings:MaxRetries");

//                await client.SetConfigurationSettingAsync("AppSettings:Timeout", "30");
//                Console.WriteLine("✓ Set: AppSettings:Timeout");

//                await client.SetConfigurationSettingAsync("ConnectionStrings:DefaultConnection", 
//                    "Server=localhost;Database=mydb;");
//                Console.WriteLine("✓ Set: ConnectionStrings:DefaultConnection");

//                await client.SetConfigurationSettingAsync("Features:NewUI", "true");
//                Console.WriteLine("✓ Set: Features:NewUI");

//                // Set with specific label
//                await client.SetConfigurationSettingAsync(
//                    new ConfigurationSetting("AppSettings:LogLevel", "Debug", "Development"));
//                Console.WriteLine("✓ Set: AppSettings:LogLevel with Development label");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error setting configuration: {ex.Message}");
//            }
//        }

//        private static async Task ReadConfigurationExample(ConfigurationClient client)
//        {
//            try
//            {
//                // Read configuration value
//                var setting = await client.GetConfigurationSettingAsync("AppSettings:MaxRetries");
//                Console.WriteLine($"✓ Read: {setting.Value.Key} = {setting.Value.Value}");

//                // Read with label
//                var labeledSetting = await client.GetConfigurationSettingAsync(
//                    "AppSettings:LogLevel", "Development");
//                Console.WriteLine($"✓ Read: {labeledSetting.Value.Key} ({labeledSetting.Value.Label}) = {labeledSetting.Value.Value}");

//                // Check if value exists
//                try
//                {
//                    await client.GetConfigurationSettingAsync("NonExistent:Key");
//                }
//                catch (RequestFailedException ex) when (ex.Status == 404)
//                {
//                    Console.WriteLine("✓ Correctly handled non-existent key");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error reading configuration: {ex.Message}");
//            }
//        }

//        private static async Task LabelsExample(ConfigurationClient client)
//        {
//            try
//            {
//                string key = "AppSettings:ApiUrl";

//                // Set values for different environments
//                await client.SetConfigurationSettingAsync(key, "https://dev-api.example.com", "Development");
//                await client.SetConfigurationSettingAsync(key, "https://staging-api.example.com", "Staging");
//                await client.SetConfigurationSettingAsync(key, "https://api.example.com", "Production");
//                await client.SetConfigurationSettingAsync(key, "https://localhost:5000", "Local");

//                Console.WriteLine($"✓ Set {key} for multiple environments");

//                // Read values for each environment
//                var environments = new[] { "Development", "Staging", "Production", "Local" };
//                foreach (var env in environments)
//                {
//                    var setting = await client.GetConfigurationSettingAsync(key, env);
//                    Console.WriteLine($"  {env}: {setting.Value.Value}");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with labels: {ex.Message}");
//            }
//        }

//        private static async Task FeatureFlagsExample(ConfigurationClient client)
//        {
//            try
//            {
//                // Feature flags are special configuration settings with key prefix ".appconfig.featureflag/"
//                string featureKey = ".appconfig.featureflag/BetaFeatures";
//                string featureValue = @"{
//                    ""id"": ""BetaFeatures"",
//                    ""enabled"": true,
//                    ""conditions"": {
//                        ""client_filters"": []
//                    }
//                }";

//                var featureSetting = new ConfigurationSetting(featureKey, featureValue)
//                {
//                    ContentType = "application/vnd.microsoft.appconfig.ff+json;charset=utf-8"
//                };

//                await client.SetConfigurationSettingAsync(featureSetting);
//                Console.WriteLine("✓ Created feature flag: BetaFeatures");

//                // Create another feature flag
//                featureKey = ".appconfig.featureflag/DarkMode";
//                featureValue = @"{
//                    ""id"": ""DarkMode"",
//                    ""enabled"": false,
//                    ""conditions"": {
//                        ""client_filters"": []
//                    }
//                }";

//                featureSetting = new ConfigurationSetting(featureKey, featureValue)
//                {
//                    ContentType = "application/vnd.microsoft.appconfig.ff+json;charset=utf-8"
//                };

//                await client.SetConfigurationSettingAsync(featureSetting);
//                Console.WriteLine("✓ Created feature flag: DarkMode");

//                // List all feature flags
//                var selector = new SettingSelector { KeyFilter = ".appconfig.featureflag/*" };
//                var featureFlags = client.GetConfigurationSettingsAsync(selector);

//                Console.WriteLine("Feature flags:");
//                await foreach (var flag in featureFlags)
//                {
//                    Console.WriteLine($"  {flag.Key.Replace(".appconfig.featureflag/", "")}: {flag.Value}");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with feature flags: {ex.Message}");
//            }
//        }

//        private static async Task ContentTypeExample(ConfigurationClient client)
//        {
//            try
//            {
//                // JSON configuration
//                var jsonSetting = new ConfigurationSetting(
//                    "AppSettings:EmailConfig",
//                    @"{""host"":""smtp.example.com"",""port"":587,""useSsl"":true}")
//                {
//                    ContentType = "application/json"
//                };
//                await client.SetConfigurationSettingAsync(jsonSetting);
//                Console.WriteLine("✓ Set JSON configuration");

//                // Plain text
//                var textSetting = new ConfigurationSetting("AppSettings:WelcomeMessage", "Welcome to our app!")
//                {
//                    ContentType = "text/plain"
//                };
//                await client.SetConfigurationSettingAsync(textSetting);
//                Console.WriteLine("✓ Set plain text configuration");

//                // Key Vault reference
//                var kvRefSetting = new ConfigurationSetting(
//                    "ConnectionStrings:Database",
//                    @"{""uri"":""https://myvault.vault.azure.net/secrets/dbconnection""}")
//                {
//                    ContentType = "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8"
//                };
//                await client.SetConfigurationSettingAsync(kvRefSetting);
//                Console.WriteLine("✓ Set Key Vault reference");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with content types: {ex.Message}");
//            }
//        }

//        private static async Task TagsExample(ConfigurationClient client)
//        {
//            try
//            {
//                var setting = new ConfigurationSetting("AppSettings:TaggedSetting", "TaggedValue");
//                setting.Tags["Environment"] = "Production";
//                setting.Tags["Owner"] = "Platform Team";
//                setting.Tags["CostCenter"] = "Engineering";
//                setting.Tags["Compliance"] = "SOC2";

//                await client.SetConfigurationSettingAsync(setting);
//                Console.WriteLine("✓ Created setting with tags");

//                // Read and display tags
//                var retrieved = await client.GetConfigurationSettingAsync("AppSettings:TaggedSetting");
//                Console.WriteLine("Tags:");
//                foreach (var tag in retrieved.Value.Tags)
//                {
//                    Console.WriteLine($"  {tag.Key}: {tag.Value}");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with tags: {ex.Message}");
//            }
//        }

//        private static async Task ListConfigurationExample(ConfigurationClient client)
//        {
//            try
//            {
//                // List all settings
//                Console.WriteLine("All configuration settings:");
//                var allSettings = client.GetConfigurationSettingsAsync(new SettingSelector());
//                int count = 0;
//                await foreach (var setting in allSettings)
//                {
//                    Console.WriteLine($"  {setting.Key} = {setting.Value}");
//                    count++;
//                    if (count >= 10) break; // Limit output
//                }
//                Console.WriteLine($"✓ Listed {count} setting(s)");

//                // Filter by key prefix
//                Console.WriteLine("\nSettings starting with 'AppSettings:'");
//                var appSettings = client.GetConfigurationSettingsAsync(
//                    new SettingSelector { KeyFilter = "AppSettings:*" });
//                count = 0;
//                await foreach (var setting in appSettings)
//                {
//                    Console.WriteLine($"  {setting.Key} = {setting.Value}");
//                    count++;
//                }
//                Console.WriteLine($"✓ Found {count} AppSettings");

//                // Filter by label
//                Console.WriteLine("\nSettings with 'Development' label:");
//                var devSettings = client.GetConfigurationSettingsAsync(
//                    new SettingSelector { LabelFilter = "Development" });
//                count = 0;
//                await foreach (var setting in devSettings)
//                {
//                    Console.WriteLine($"  {setting.Key} = {setting.Value}");
//                    count++;
//                }
//                Console.WriteLine($"✓ Found {count} Development settings");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error listing configuration: {ex.Message}");
//            }
//        }

//        private static async Task SnapshotsExample(ConfigurationClient client)
//        {
//            try
//            {
//                // Note: Snapshots are a premium feature
//                // This is a conceptual example
//                Console.WriteLine("Snapshot functionality (requires premium tier):");
//                Console.WriteLine("  - Create point-in-time snapshots of configuration");
//                Console.WriteLine("  - Roll back to previous configurations");
//                Console.WriteLine("  - Compare configurations across time");
//                Console.WriteLine("✓ Snapshot concepts demonstrated");

//                await Task.CompletedTask;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with snapshots: {ex.Message}");
//            }
//        }

//        private static async Task KeyVaultReferencesExample(ConfigurationClient client)
//        {
//            try
//            {
//                // Create a Key Vault reference
//                // Format: {"uri":"https://<key-vault-name>.vault.azure.net/secrets/<secret-name>"}
//                var kvReference = new ConfigurationSetting(
//                    "Database:ConnectionString",
//                    @"{""uri"":""https://your-keyvault.vault.azure.net/secrets/SqlConnectionString""}")
//                {
//                    ContentType = "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8"
//                };

//                await client.SetConfigurationSettingAsync(kvReference);
//                Console.WriteLine("✓ Created Key Vault reference for database connection string");

//                // Create another Key Vault reference
//                var apiKeyReference = new ConfigurationSetting(
//                    "ExternalApi:ApiKey",
//                    @"{""uri"":""https://your-keyvault.vault.azure.net/secrets/ThirdPartyApiKey""}")
//                {
//                    ContentType = "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8"
//                };

//                await client.SetConfigurationSettingAsync(apiKeyReference);
//                Console.WriteLine("✓ Created Key Vault reference for API key");

//                Console.WriteLine("\nKey Vault references allow you to:");
//                Console.WriteLine("  - Store sensitive values in Key Vault");
//                Console.WriteLine("  - Reference them from App Configuration");
//                Console.WriteLine("  - Benefit from Key Vault's security features");
//                Console.WriteLine("  - Centralize secret management");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with Key Vault references: {ex.Message}");
//            }
//        }

//        private static async Task IConfigurationIntegrationExample(string connectionString)
//        {
//            try
//            {
//                Console.WriteLine("Integration with Microsoft.Extensions.Configuration:");

//                // Build configuration with Azure App Configuration
//                var builder = new ConfigurationBuilder();
//                builder.AddAzureAppConfiguration(options =>
//                {
//                    options.Connect(connectionString)
//                           .Select("AppSettings:*")
//                           .Select("ConnectionStrings:*")
//                           .ConfigureRefresh(refresh =>
//                           {
//                               refresh.Register("AppSettings:Sentinel", refreshAll: true)
//                                      .SetCacheExpiration(TimeSpan.FromSeconds(30));
//                           });
//                });

//                var configuration = builder.Build();

//                // Access configuration values
//                Console.WriteLine("✓ Built IConfiguration from App Configuration");
//                Console.WriteLine("  Configuration can be injected into ASP.NET Core apps");
//                Console.WriteLine("  Supports automatic refresh");
//                Console.WriteLine("  Works with Options pattern");

//                // Example of accessing values (these may not exist)
//                var maxRetries = configuration["AppSettings:MaxRetries"];
//                if (maxRetries != null)
//                {
//                    Console.WriteLine($"  Sample value - MaxRetries: {maxRetries}");
//                }

//                await Task.CompletedTask;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"⚠ Error with IConfiguration integration: {ex.Message}");
//            }
//        }

//        #endregion

//        #region Helper Methods

//        private static string MaskSecret(string secret)
//        {
//            if (string.IsNullOrEmpty(secret) || secret.Length <= 4)
//                return "****";

//            return secret.Substring(0, 2) + new string('*', secret.Length - 4) + secret.Substring(secret.Length - 2);
//        }

//        #endregion
//    }
//}
