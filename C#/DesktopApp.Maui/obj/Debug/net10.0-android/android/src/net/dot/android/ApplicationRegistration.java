package net.dot.android;

public class ApplicationRegistration {

	public static android.content.Context Context;

	public static void registerApplications ()
	{
		// Application and Instrumentation ACWs must be registered first.
		mono.android.Runtime.register ("DesktopApp.Maui.MainApplication, DesktopApp.Maui, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", crc64306082d1a2e81f92.MainApplication.class, crc64306082d1a2e81f92.MainApplication.__md_methods);
		mono.android.Runtime.register ("Microsoft.Maui.MauiApplication, Microsoft.Maui, Version=10.0.0.0, Culture=neutral, PublicKeyToken=null", crc6488302ad6e9e4df1a.MauiApplication.class, crc6488302ad6e9e4df1a.MauiApplication.__md_methods);
		
	}
}
