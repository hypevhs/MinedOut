#version 120

uniform sampler2D font;
uniform sampler2D foredata;
uniform sampler2D backdata;
uniform vec2 buffersize;
uniform vec4 fontsizes;

void main() {
	vec4 Fore = texture2D(foredata, gl_TexCoord[0].xy);
	vec4 Back = texture2D(backdata, gl_TexCoord[0].xy);
	float chr = 255.0f * Fore.a;
	
	vec2 fontpos = vec2(floor(mod(chr, fontsizes.z)) * fontsizes.x, floor(chr / fontsizes.w) * fontsizes.y);
	vec2 offset = vec2(mod(floor(gl_TexCoord[0].x * (buffersize.x * fontsizes.x)), fontsizes.x),
					   mod(floor(gl_TexCoord[0].y * (buffersize.y * fontsizes.y)) + 0.5f, fontsizes.y));

	vec4 fontclr = texture2D(font, (fontpos + offset) / vec2(fontsizes.x * fontsizes.z, fontsizes.y * fontsizes.w));
	gl_FragColor = mix(Back, vec4(Fore.rgb, 1.0f), fontclr.r);
}
